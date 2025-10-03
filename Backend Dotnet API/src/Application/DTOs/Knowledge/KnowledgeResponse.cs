using Application.DTOs.Agent;
using Domain.Enums;

namespace Application.DTOs.Knowledge;

public class KnowledgeResponse
{
    public Guid IdKnowledge { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Origin? Origin { get; set; }
}