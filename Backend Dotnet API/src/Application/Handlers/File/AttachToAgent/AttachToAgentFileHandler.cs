using Application.Contracts.Response;
using Application.DTOs.File;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;
using System.IO;
using System.Linq;

namespace Application.Handlers.File.AttachToAgent;

public class AttachToAgentFileHandler : BaseHandler
{
    private readonly IFileRepository _fileRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IGemelliAIService _gemelliAIService;

    public AttachToAgentFileHandler(IFileRepository fileRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IAgentRepository agentRepository,
        IFileUploadService fileUploadService,
        IGemelliAIService gemelliAIService)
    {
        _fileRepository = fileRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _agentRepository = agentRepository;
        _fileUploadService = fileUploadService;
        _gemelliAIService = gemelliAIService;
    }

    public async Task<ErrorOr<FileResponse>> Handle(AttachToAgentFileRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new AttachToAgentFileRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (!_moduleService.HasAccessToModule(user, module))
        {
            return UserErrors.ModuleAccessDenied;
        }

        Domain.Entities.File? file = await _fileRepository.SearchByIdAsync(request.IdFile, module, cancellationToken);

        if (file is null)
        {
            return FileErrors.NotFound;
        }

        Domain.Entities.Agent agent = await _agentRepository.SearchByIdAsync(request.IdAgent, user.Organizations.FirstOrDefault()!, module, cancellationToken);

        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        if (file.HasAgent(request.IdAgent))
        {
            file.RemoveAgent(agent.Id);
        }
        else
        {
            ErrorOr<Stream> blobStreamResult = await _fileUploadService.OpenReadAsync(
                file.FileName,
                cancellationToken);

            if (blobStreamResult.IsError)
            {
                return blobStreamResult.Errors;
            }

            await using Stream blobStream = blobStreamResult.Value;

            string organization = user.Organizations.FirstOrDefault()!;

            ErrorOr<GemelliAIFileResponse> aiResponse = await _gemelliAIService.FileAsync(
                new GemelliAIFileRequest
                {
                    FileStream = blobStream,
                    FileName = file.FileName,
                    Organization = organization,
                    IdAgent = agent.Id.ToString(),
                    IdFile = file.Id.ToString(),
                },
                cancellationToken);

            if (aiResponse.IsError)
            {
                return aiResponse.Errors;
            }

            file.AddAgent(agent);
            file.Resume = aiResponse.Value.Resume;
            file.GeneratedName = aiResponse.Value.FileName;
        }

        _fileRepository.Update(file);
        await _fileRepository.UnitOfWork.Commit();

        FileResponse fileResponse = file.Adapt<FileResponse>();
        return fileResponse;
    }
}