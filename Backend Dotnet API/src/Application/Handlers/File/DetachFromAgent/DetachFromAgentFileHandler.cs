using Application.DTOs.File;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;
using System.Linq;

namespace Application.Handlers.File.DetachFromAgent;

public class DetachFromAgentFileHandler : BaseHandler
{
    private readonly IFileRepository _fileRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IGemelliAIService _gemelliAIService;

    public DetachFromAgentFileHandler(IFileRepository fileRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IAgentRepository agentRepository,
        IGemelliAIService gemelliAIService)
    {
        _fileRepository = fileRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _agentRepository = agentRepository;
        _gemelliAIService = gemelliAIService;
    }

    public async Task<ErrorOr<FileResponse>> Handle(DetachFromAgentFileRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new DetachFromAgentFileRequestValidator()) is var resultado && resultado.Any())
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
            string organization = user.Organizations.FirstOrDefault()!;

            ErrorOr<bool> deletionResult = await _gemelliAIService.DeleteFileAsync(
                organization,
                agent.Id.ToString(),
                file.Id.ToString(),
                cancellationToken);

            if (deletionResult.IsError)
            {
                return deletionResult.Errors;
            }

            if (!deletionResult.Value)
            {
                return FileErrors.DeleteFail;
            }

            file.RemoveAgent(agent.Id);
            _fileRepository.Update(file);
            await _fileRepository.UnitOfWork.Commit();
        }

        FileResponse fileResponse = file.Adapt<FileResponse>();
        return fileResponse;
    }
}
