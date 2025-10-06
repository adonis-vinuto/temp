using Application.Contracts.Response;
using Application.DTOs.File;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.File.Remove;

public sealed class RemoveFileHandler : BaseHandler
{
    private readonly IFileRepository _fileRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IFileUploadService _fileUploadService;

    public RemoveFileHandler(IFileRepository fileRepository,
        IAuthenticationService authenticationService,
        IFileUploadService fileUploadService)
    {
        _fileRepository = fileRepository;
        _authenticationService = authenticationService;
        _fileUploadService = fileUploadService;
    }

    public async Task<ErrorOr<FileResponse>> Handle(RemoveFileRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new RemoveFileRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.File? file = await _fileRepository.SearchByIdAsync(request.Id, module, cancellationToken);

        if (file is null)
        {
            return FileErrors.NotFound;
        }

        ErrorOr<bool> deleteResult = await _fileUploadService.DeleteAsync(file.FileName, cancellationToken);
        if (deleteResult.IsError)
        {
            return FileErrors.DeleteFail;
        }

        _fileRepository.Remove(file);
        await _fileRepository.UnitOfWork.Commit();

        FileResponse fileResponse = file.Adapt<FileResponse>();
        return fileResponse;
    }
}