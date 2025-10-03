using Application.Common.Responses;
using Application.DTOs.File;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.File.Search;

public class SearchFilesHandler : BaseHandler
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IFileRepository _fileRepository;
    private readonly IFileUploadService _fileUploadService;

    public SearchFilesHandler(
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IFileRepository fileRepository,
        IFileUploadService fileUploadService)
    {
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _fileRepository = fileRepository;
        _fileUploadService = fileUploadService;
    }

    public async Task<ErrorOr<PagedResponse<FileResponse>>> Handle(SearchFilesRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new SearchFilesRequestValidator()) is var resultado && resultado.Any())
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

        PagedResponse<Domain.Entities.File> pagedFiles = await _fileRepository.PagedSearchAsync(module, request.IdAgent, request.Pagina, request.TamanhoPagina, cancellationToken);
        
        ErrorOr<List<(string name, Uri url)>> blobs = _fileUploadService.ListAsync(
                pagedFiles.Itens.Select(f => f.FileName).ToList(),
                cancellationToken
            );

        List<(string name, Uri url)> blobsResult = blobs.Value;
        
        var urlDict = blobsResult.ToDictionary(x => x.name, x => x.url);

        var filesResponse = pagedFiles.Itens
            .Select(file =>
            {
                FileResponse response = file.Adapt<FileResponse>();
                if (urlDict.TryGetValue(file.FileName, out Uri? url))
                {
                    response.UrlFile = url.ToString();
                }
                return response;
            })
            .ToList();

        return PagedResponse<FileResponse>.Create(
            filesResponse,
            pagedFiles.TotalItens,
            pagedFiles.Indice,
            pagedFiles.TamanhoPagina
        );
    }
}