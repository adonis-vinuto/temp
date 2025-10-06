using Application.Interfaces.Data;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Application.Common.Responses;
using Domain.Entities;
using Application.Interfaces.Repositories;
using Domain.Enums;

namespace Infrastructure.Repositories;

public class DataConfigRepository : IDataConfigRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly GemelliApiContext _context;

    public DataConfigRepository(GemelliApiContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DataConfig dataConfig, CancellationToken cancellationToken)
    {
        await _context.DataConfigs.AddAsync(dataConfig, cancellationToken);
    }

    public async Task<PagedResponse<DataConfig>> Search(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<DataConfig> query = _context.DataConfigs;


        int totalItens = await query.CountAsync(cancellationToken);

        List<DataConfig> itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<DataConfig>.Create(
            itens,
            totalItens,
            pagina,
            tamanhoPagina
        );
    }

    public async Task<DataConfig?> SearchByIdAsync(Guid idDataConfig, string organization, CancellationToken cancellationToken)
    {
        return await _context.DataConfigs.FirstOrDefaultAsync(x => x.Id == idDataConfig && x.Organization == organization, cancellationToken);
    }
    
    public async Task<DataConfig?> GetDataConfigByOrganization(string organization, CancellationToken cancellationToken)
    {
        return await _context.DataConfigs.FirstOrDefaultAsync(x => x.Organization == organization, cancellationToken);
    }

    public void Edit(DataConfig dataConfig)
    {
        _context.DataConfigs.Update(dataConfig);
    }
    
    public async Task<PagedResponse<DataConfig>> PagedSearchAsync(
        string organization,
        Module? module,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<DataConfig> query = _context.DataConfigs
            .Where(x => x.Organization == organization);

        if (module.HasValue)
        {
            query = query.Where(x => x.Module == module.Value);
        }

        int totalItens = await query.CountAsync(cancellationToken);

        List<DataConfig> itens = await query
            .OrderBy(a => a.CreatedAt)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<DataConfig>.Create(
            itens,
            totalItens,
            pagina,
            tamanhoPagina
        );
    }
    
    public async Task<string?> GetTenantConnectionStringAsync(string organizationName)
    {
        DataConfig? config = await _context.DataConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Organization == organizationName);

        if (config == null)
        {
            return null;
        }

        string connectionString =
            $"Host={config.SqlHost};" +
            $"Port={config.SqlPort};" +
            $"Database={config.SqlDatabase};" +
            $"Username={config.SqlUser};" +
            $"Password={config.SqlPassword};";

        return connectionString;
    }
}
