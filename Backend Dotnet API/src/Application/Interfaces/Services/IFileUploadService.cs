using ErrorOr;
using Application.Contracts.Response;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Application.Interfaces.Services;

public interface IFileUploadService
{
    Task<ErrorOr<Uri>> GetAsync(string filename, CancellationToken cancellationToken);
    public ErrorOr<List<(string name, Uri url)>> ListAsync(List<string> filenames, CancellationToken cancellationToken);
    Task<ErrorOr<bool>> DeleteAsync(string nomeArquivo, CancellationToken cancellationToken);
    Task<ErrorOr<FileResponseModel>> UploadAsync(
        IFormFile inputFile,
        CancellationToken cancellationToken
    );
    Task<ErrorOr<Stream>> OpenReadAsync(string filename, CancellationToken cancellationToken);
}
