using Domain.Enums;

namespace Application.DTOs.Agent;

public class AiAgentResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public TypeAgent TypeAgent { get; set; }
}