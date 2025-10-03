using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ErrorOr;
using Application.Contracts.Response;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Sas;

namespace Infrastructure.Services;

public class FileUploadService : IFileUploadService
{
    private readonly BlobContainerClient _filesContainer;
    private readonly string _containerName;


    public FileUploadService(IConfiguration configuration)
    {
        _filesContainer = new BlobContainerClient(configuration.GetConnectionString("GemelliApiBlobConnection")!,
        configuration.GetConnectionString("GemelliApiBlobContainerName"));
        _containerName = configuration.GetConnectionString("GemelliApiBlobContainerName")!;
    }

    public async Task<ErrorOr<Uri>> GetAsync(string filename, CancellationToken cancellationToken)
    {
        BlobClient client = _filesContainer.GetBlobClient(filename);

        if (!await client.ExistsAsync(cancellationToken))
        {
            throw new InvalidOperationException("Error getting Blob Client.");
        }

        if (!client.CanGenerateSasUri)
        {
            throw new InvalidOperationException("Cannot generate SAS for this blob.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = filename,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        
        return client.GenerateSasUri(sasBuilder);
    }

    public ErrorOr<List<(string name, Uri url)>> ListAsync(List<string> filenames, CancellationToken cancellationToken)
    {
        var imageUrls = filenames.Select(filename =>
        {
            BlobClient blobClient = _filesContainer.GetBlobClient(filename);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = filename,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            Uri uri = blobClient.GenerateSasUri(sasBuilder);
            return (filename, uri);
        }).ToList();

        return imageUrls;
    }

    public async Task<ErrorOr<FileResponseModel>> UploadAsync(
        IFormFile inputFile,
        CancellationToken cancellationToken
    )
    {
        BlobClient createClient = _filesContainer.GetBlobClient(Guid.NewGuid() + "-" + inputFile.FileName);

        await using (Stream data = inputFile.OpenReadStream())
        {
            await createClient.UploadAsync(
                data,
                new BlobHttpHeaders { ContentType = inputFile.ContentType },
                cancellationToken: cancellationToken
            );
        }

        var fileResponse = new FileResponseModel(
            createClient.Uri.AbsoluteUri,
            createClient.Name,
            inputFile.ContentType
        );

        return fileResponse;
    }

    public async Task<ErrorOr<bool>> DeleteAsync(string nomeArquivo, CancellationToken cancellationToken)
    {
        string lastNomeArquivo = Uri.UnescapeDataString(nomeArquivo.Split("/")[^1]);
        BlobClient blob = _filesContainer.GetBlobClient(lastNomeArquivo);
        await blob.DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: cancellationToken
        );
        return true;
    }
}
