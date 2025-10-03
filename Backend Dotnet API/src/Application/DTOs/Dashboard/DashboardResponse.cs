namespace Application.DTOs.Dashboard;

public class DashboardResponse
{
    public int TotalAgents { get; set; }
    public int TotalInteractions { get; set; }
    public List<InteractionsByAgentType> InteractionsByAgentType { get; set; }
}

public class InteractionsByAgentType
{
    public int AgentType { get; set; }
    public int InteractionsCount { get; set; }
}