using Domain.Enums;

namespace Domain.Entities;

public sealed class DataConfig : BaseEntity
{

    public Module Module { get; set; }
    public string Organization { get; set; }
    public string SqlHost { get; set; }
    public string SqlPort { get; set; }
    public string SqlUser { get; set; }
    public string SqlPassword { get; set; }
    public string SqlDatabase { get; set; }
    public string BlobConnectionString { get; set; }
    public string BlobContainerName { get; set; }
    
    public DataConfig() { }
    public DataConfig( Module module, string organization, string sqlHost, string sqlPort, string sqlUser, string sqlPassword, string sqlDatabase, string? blobConnectionString, string? blobContainerName)
    {
        Module = module;
        Organization = organization;
        SqlHost = sqlHost;
        SqlPort = sqlPort;
        SqlUser = sqlUser;
        SqlPassword = sqlPassword;
        SqlDatabase = sqlDatabase;
        BlobConnectionString = blobConnectionString!;
        BlobContainerName = blobContainerName!;
    }
}