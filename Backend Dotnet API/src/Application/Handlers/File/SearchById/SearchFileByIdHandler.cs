using ErrorOr;
using MapsterMapper;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using Application.Contracts.Response;
using Application.DTOs.File;

namespace Application.Handlers.File.SearchById;

public class SearchFileByIdHandler : BaseHandler
{
    private readonly IFileRepository _fileRepository;
    private readonly IFileUploadService _fileUploadService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public SearchFileByIdHandler(IFileRepository fileRepository, IAuthenticationService authenticationService, IFileUploadService fileUploadService, IMapper mapper)
    {
        _fileRepository = fileRepository;
        _authenticationService = authenticationService;
        _fileUploadService = fileUploadService;
        _mapper = mapper;
    }

    public async Task<ErrorOr<FileResponse>> Handle(SearchFileByIdRequest request, Module module, CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchFileByIdRequestValidator()) is var resultado && resultado.Any())
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

        ErrorOr<Uri> sasUri = await _fileUploadService.GetAsync(file.FileName, cancellationToken);

        if (sasUri.IsError)
        {
            return FileErrors.NotFound;
        }

        FileResponse response = _mapper.Map<FileResponse>(file);
        response.UrlFile = sasUri.Value.ToString();

        return response;
    }
}