namespace Application.DTOs.DataConfig;

public class DataConfigResponse
{
    public Guid Id { get; set; }
    public string Organization { get; set; }
    public string SqlHost { get; set; } = string.Empty;
    public string SqlPort { get; set; } = string.Empty;
    public string SqlUser { get; set; } = string.Empty;
    public string SqlPassword { get; set; } = string.Empty;
    public string SqlDatabase { get; set; } = string.Empty;
    public string BlobConnectionString { get; set; } = string.Empty;
    public string BlobContainerName { get; set; } = string.Empty;
}