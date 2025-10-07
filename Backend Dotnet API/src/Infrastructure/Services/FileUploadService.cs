using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ErrorOr;
using Application.Contracts.Response;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Sas;
using System.IO;

namespace Infrastructure.Services;

public class FileUploadService : IFileUploadService
{
    private readonly BlobContainerClient _filesContainer;
    private readonly string _containerName;
    private static readonly Error BlobNotFoundError = Error.NotFound(
        code: "File.Blob.NotFound",
        description: "Arquivo não encontrado no armazenamento.");

    private static readonly Error BlobReadError = Error.Failure(
        code: "File.Blob.ReadError",
        description: "Não foi possível abrir o arquivo para leitura.");


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
        string originalFileName = Path.GetFileName(inputFile.FileName);

        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new InvalidOperationException("Nome do arquivo inválido para upload.");
        }

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        string extension = Path.GetExtension(originalFileName);

        string fileNameToUse = originalFileName;
        BlobClient createClient = _filesContainer.GetBlobClient(fileNameToUse);

        int duplicateCounter = 1;
        while (await createClient.ExistsAsync(cancellationToken))
        {
            string suffix = duplicateCounter.ToString();
            fileNameToUse = string.IsNullOrEmpty(fileNameWithoutExtension)
                ? $"{suffix}{extension}"
                : $"{fileNameWithoutExtension}_{suffix}{extension}";

            createClient = _filesContainer.GetBlobClient(fileNameToUse);
            duplicateCounter++;
        }

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

    public async Task<ErrorOr<Stream>> OpenReadAsync(string filename, CancellationToken cancellationToken)
    {
        BlobClient blobClient = _filesContainer.GetBlobClient(filename);

        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            return BlobNotFoundError;
        }

        try
        {
            return await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        }
        catch (NotSupportedException)
        {
            return await DownloadAsStreamAsync(blobClient, cancellationToken);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            return BlobNotFoundError;
        }
        catch (RequestFailedException)
        {
            return await DownloadAsStreamAsync(blobClient, cancellationToken);
        }
        catch
        {
            return BlobReadError;
        }
    }

    private static async Task<ErrorOr<Stream>> DownloadAsStreamAsync(
        BlobClient blobClient,
        CancellationToken cancellationToken)
    {
        try
        {
            BlobDownloadStreamingResult download = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return download.Content;
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            return BlobNotFoundError;
        }
        catch
        {
            return BlobReadError;
        }
    }
}
