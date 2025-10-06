namespace Application.Contracts.Response;

public record FileResponseModel(
    Uri? Uri,
    string? Name,
    string? ContentType
)
{
    // Construtor adicional que aceita uma string e converte para Uri
    public FileResponseModel(string? uri, string? name, string? contentType)
        : this(uri is not null ? new Uri(uri) : null, name, contentType)
    {
    }
}
