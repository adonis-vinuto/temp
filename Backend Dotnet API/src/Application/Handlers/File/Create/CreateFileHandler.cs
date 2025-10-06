using Application.Contracts.Response;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using System;

namespace Application.Handlers.File.Create;

public class CreateFileHandler : BaseHandler
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IFileRepository _fileRepository;

    public CreateFileHandler(
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IFileUploadService fileUploadService,
        IFileRepository fileRepository)
    {
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _fileUploadService = fileUploadService;
        _fileRepository = fileRepository;
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

        await _fileRepository.AddAsync(new Domain.Entities.File()
        {
            Id = fileId,
            FileName = arquivo.Value.Name!,
            Module = module,
            Resume = null,
        }, cancellationToken);
        await _fileRepository.UnitOfWork.Commit();

        return arquivo;
    }
}