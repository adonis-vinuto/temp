using Application.DTOs.Agent;
using Application.DTOs.Page;

namespace Application.DTOs.File;

public class FileResponse
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string? GeneratedName { get; set; }
    public string UrlFile { get; set; }
    public IEnumerable<PageResponse>? Pages { get; set; }
    public IEnumerable<AgentResponse>? Agents { get; set; }
}