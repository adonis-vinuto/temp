using Domain.Enums;

namespace Application.DTOs.Agent;

public class AgentResponse
{
    public Guid Id { get; set; }
    public string Organization { get; set; }
    public Module Module { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public TypeAgent TypeAgent { get; set; }
    public Guid IdUser { get; set; }
}