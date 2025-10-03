
namespace Domain.Entities;

public class SeniorHcmConfig : BaseEntity
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? WsdlUrl { get; set; }
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
