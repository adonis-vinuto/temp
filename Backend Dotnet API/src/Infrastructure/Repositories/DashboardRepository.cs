using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Application.DTOs.Dashboard;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public DashboardRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponse> GetDashboardAsync(
        Module module,
        CancellationToken cancellationToken)
    {
        int totalAgents = await _context.Agents
            .AsNoTracking()
            .Where(x => x.Module == module)
            .CountAsync(cancellationToken);

        int totalInteractions = await _context.Chats
            .AsNoTracking()
            .Where(cs => _context.Agents
                .Where(a => a.Module == module)
                .Select(a => a.Id)
                .Contains(cs.IdAgent))
            .SumAsync(cs => (int?)cs.TotalInteractions, cancellationToken) ?? 0;

        var interactionsByAgentType = await _context.Agents
            .AsNoTracking()
            .Where(a => a.Module == module)
            .GroupJoin(
                _context.Chats,
                agent => agent.Id,
                chat => chat.IdAgent,
                (agent, chats) => new
                {
                    agent.Type,
                    InteractionsCount = chats.Sum(c => (int?)c.TotalInteractions ?? 0)
                }
            )
            .ToListAsync(cancellationToken);

        return new DashboardResponse
        {
            TotalAgents = totalAgents,
            TotalInteractions = totalInteractions,
            InteractionsByAgentType = interactionsByAgentType
                .Select(x => new InteractionsByAgentType
                {
                    AgentType = (int)x.Type,
                    InteractionsCount = x.InteractionsCount
                })
                .ToList()
        };
    }
}
