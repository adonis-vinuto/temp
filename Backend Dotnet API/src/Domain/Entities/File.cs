using Domain.Enums;

namespace Domain.Entities;

public sealed class File : BaseEntity
{
    public Module Module { get; set; }
    public string FileName { get; set; }
    public string? Resume { get; set; }
    public int? CompletionTokens { get; set; }
    public int? PromptTokens { get; set; }
    public int? TotalTokens { get; set; }
    public int? CompletionTime { get; set; }
    public int? PromptTime { get; set; }
    public int? QueueTime { get; set; }
    public int? TotalTime { get; set; }
    public IEnumerable<Agent>? Agents => _agents;
    private readonly List<Agent> _agents = [];
    public bool HasAgent(Guid idAgent)

    {
        return _agents.Any(x => x.Id == idAgent);
    }

    public void AddAgent(Agent agent)
    {
        _agents.Add(agent);
    }

    public void RemoveAgent(Guid agentId)
    {
        Agent? existingAgent = _agents.Find(x => x.Id == agentId);
        if (existingAgent is not null)
        {
            _agents.Remove(existingAgent);
        }
    }


}
