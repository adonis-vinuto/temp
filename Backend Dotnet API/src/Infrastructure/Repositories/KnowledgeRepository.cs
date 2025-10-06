using Application.Common.Responses;
using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class KnowledgeRepository : IKnowledgeRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public KnowledgeRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Knowledge knowledge, CancellationToken cancellationToken)
    {
        await _context.Knowledges.AddAsync(knowledge, cancellationToken);
    }

    public async Task<Knowledge?> SearchByIdAsync(Guid idKnowledge, Module module, CancellationToken cancellationToken)
    {
        return await _context.Knowledges
            .Include(x => x.Agents)
            .FirstOrDefaultAsync(
                x => x.Id == idKnowledge &&
                 x.Module == module, cancellationToken
            );
    }

    public void Update(Knowledge knowledge)
    {
        _context.Knowledges.Update(knowledge);
    }

    public void Remove(Knowledge knowledge)
    {
        _context.Employees.RemoveRange(_context.Employees);
        _context.SeniorErpConfigs.RemoveRange(_context.SeniorErpConfigs);
        _context.SeniorHcmConfigs.RemoveRange(_context.SeniorHcmConfigs);
        _context.Knowledges.Remove(knowledge);
    }

    public async Task<PagedResponse<Knowledge>> PagedSearchAsync(
        Module module,
        Guid? idAgent,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<Knowledge> query = _context.Knowledges
            .Include(x => x.Agents)
            .AsNoTracking().Where(x => x.Module == module);

        if (idAgent != null)
        {
            query = query.Where(x => x.Agents != null && x.Agents.Any(x => x.Id == idAgent));
        }

        int totalItens = await query.CountAsync(cancellationToken);

        List<Knowledge> itens = await query
            .OrderBy(a => a.CreatedAt)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<Knowledge>.Create(
            itens,
            totalItens,
            pagina,
            tamanhoPagina
        );
    }
}
