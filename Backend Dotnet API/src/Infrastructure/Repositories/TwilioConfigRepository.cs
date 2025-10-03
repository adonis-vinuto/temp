using Application.Common.Responses;
using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TwilioConfigRepository : ITwilioConfigRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public TwilioConfigRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TwilioConfig twilioConfig, CancellationToken cancellationToken)
    {
        await _context.TwilioConfigs.AddAsync(twilioConfig, cancellationToken);
    }

    public async Task<TwilioConfig?> SearchByIdAsync(Guid id, Module module, CancellationToken cancellationToken)
    {
        return await _context.TwilioConfigs
            .Include(x => x.Agent)
            .FirstOrDefaultAsync(x => x.Id == id && x.Agent.Module == module, cancellationToken);
    }

    public async Task<TwilioConfig?> SearchByAgentIdAsync(Guid idAgent, Module module, CancellationToken cancellationToken)
    {
        return await _context.TwilioConfigs
            .Include(x => x.Agent)
            .FirstOrDefaultAsync(x => x.IdAgent == idAgent && x.Agent.Module == module, cancellationToken);
    }

    public async Task<PagedResponse<TwilioConfig>> PagedSearchAsync(
        Module module,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<TwilioConfig> query = _context.TwilioConfigs
            .Include(x => x.Agent)
            .Where(x => x.Agent.Module == module);

        int totalItens = await query.CountAsync(cancellationToken);

        List<TwilioConfig> itens = await query
            .OrderBy(x => x.CreatedAt)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<TwilioConfig>.Create(
            itens,
            totalItens,
            pagina,
            tamanhoPagina
        );
    }

    public void Edit(TwilioConfig twilioConfig)
    {
        _context.TwilioConfigs.Update(twilioConfig);
    }

    public void Remove(TwilioConfig twilioConfig)
    {
        _context.TwilioConfigs.Remove(twilioConfig);
    }
}
