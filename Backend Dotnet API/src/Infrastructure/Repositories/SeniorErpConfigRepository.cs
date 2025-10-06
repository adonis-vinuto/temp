using Application.Common.Responses;
using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SeniorErpConfigRepository : ISeniorErpConfigRepository
{
    public IUnitOfWork UnitOfWork => _context; 
    private readonly TenantDbContext _context; 

    public SeniorErpConfigRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SeniorErpConfig config, CancellationToken cancellationToken) 
    {
        await _context.SeniorErpConfigs.AddAsync(config, cancellationToken);
    }

    public async Task<SeniorErpConfig?> SearchByIdAsync(Guid idConfig, CancellationToken cancellationToken)
    {
        return await _context.SeniorErpConfigs
            .Include(x => x.Agents)
            .FirstOrDefaultAsync(x => x.Id == idConfig, cancellationToken);
    }

    public void Edit(SeniorErpConfig config) 
    {
        _context.SeniorErpConfigs.Update(config);
    }
    
    public async Task<PagedResponse<SeniorErpConfig>> PagedSearchAsync( 
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        
        IQueryable<SeniorErpConfig> query = _context.SeniorErpConfigs.AsQueryable();
        
        int totalItens = await query.CountAsync(cancellationToken);
        
        List<SeniorErpConfig> items = await query
            .Include(x => x.Agents)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<SeniorErpConfig>.Create(
            items, 
            totalItens, 
            pagina,
            tamanhoPagina
        );
    }

    public void Remove(SeniorErpConfig config)
    {
        _context.SeniorErpConfigs.Remove(config);
    }
    
    public void Update(SeniorErpConfig seniorErpConfig)
    {
        _context.SeniorErpConfigs.Update(seniorErpConfig);
    }

}
