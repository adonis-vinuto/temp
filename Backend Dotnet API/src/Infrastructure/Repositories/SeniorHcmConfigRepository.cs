using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Responses;
using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SeniorHcmConfigRepository : ISeniorHcmConfigRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public SeniorHcmConfigRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SeniorHcmConfig config, CancellationToken cancellationToken)
    {
        await _context.SeniorHcmConfigs.AddAsync(config, cancellationToken);
    }

    public async Task<SeniorHcmConfig?> SearchByIdAsync(Guid idConfig, CancellationToken cancellationToken)
    {
        return await _context.SeniorHcmConfigs
            .Include(x => x.Agents)
            .FirstOrDefaultAsync(x => x.Id == idConfig, cancellationToken);
    }

    public void Edit(SeniorHcmConfig config)
    {
        _context.SeniorHcmConfigs.Update(config);
    }

    public async Task<PagedResponse<SeniorHcmConfig>> PagedSearchAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<SeniorHcmConfig> query = _context.SeniorHcmConfigs.AsQueryable();

        int totalItens = await query.CountAsync(cancellationToken);

        List<SeniorHcmConfig> items = await query
            .Include(x => x.Agents)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<SeniorHcmConfig>.Create(
            items,
            totalItens,
            pagina,
            tamanhoPagina);
    }

    public void Remove(SeniorHcmConfig config)
    {
        _context.SeniorHcmConfigs.Remove(config);
    }
    
    public void Update(SeniorHcmConfig seniorHcmConfig)
    {
        _context.SeniorHcmConfigs.Update(seniorHcmConfig);
    }
   
}
