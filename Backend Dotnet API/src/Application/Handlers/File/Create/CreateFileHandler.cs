using Application.Contracts.Response;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using System;
using System.IO;

namespace Application.Handlers.File.Create;

public class CreateFileHandler : BaseHandler
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IFileRepository _fileRepository;
    private readonly IGemelliAIService _gemelliAIService;

    public CreateFileHandler(
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IFileUploadService fileUploadService,
        IFileRepository fileRepository,
        IGemelliAIService gemelliAIService)
    {
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _fileUploadService = fileUploadService;
        _fileRepository = fileRepository;
        _gemelliAIService = gemelliAIService;
    }

    public async Task<ErrorOr<FileResponseModel>> Handle(CreateFileRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateFileRequestValidator()) is var resultado && resultado.Any())
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

        if (request.Arquivo == null)
        {
            return FileErrors.NotFound; 
        }

        Guid fileId = request.IdFile ?? Guid.NewGuid();

        ErrorOr<FileResponseModel> arquivo = await _fileUploadService.UploadAsync(request.Arquivo, cancellationToken);

        if (arquivo.IsError)
        {
            return FileErrors.UploadFail;
        }

        string? generatedName = null;
        string? resume = null;

        await using Stream summaryStream = request.Arquivo.OpenReadStream();

        ErrorOr<GemelliAIFileResponse> summaryResult = await _gemelliAIService.SummarizeFileAsync(
            summaryStream,
            request.Arquivo.FileName,
            request.Arquivo.ContentType ?? "application/octet-stream",
            cancellationToken);

        if (!summaryResult.IsError)
        {
            generatedName = summaryResult.Value.FileName;
            resume = summaryResult.Value.Resume;
        }

        await _fileRepository.AddAsync(new Domain.Entities.File()
        {
            Id = fileId,
            FileName = arquivo.Value.Name!,
            Module = module,
            GeneratedName = generatedName,
            Resume = resume,
        }, cancellationToken);
        await _fileRepository.UnitOfWork.Commit();

        return arquivo;
    }
}