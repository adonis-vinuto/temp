using Domain.Enums;

namespace Domain.Entities;

public sealed class Knowledge : BaseEntity
{
    public Module Module { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Origin? Origin { get; set; }
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

    public void RemoveAgent(Agent agent)
    {
        _agents.Remove(agent);
    }
}