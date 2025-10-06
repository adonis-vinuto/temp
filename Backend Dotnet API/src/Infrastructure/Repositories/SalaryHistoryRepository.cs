using Application.Common.Responses;
using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SalaryHistoryRepository : ISalaryHistoryRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public SalaryHistoryRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<SalaryHistory>> SearchSalaryHistoryByIdKnowledge(Guid idKnowledge, int pagina, int tamanhoPagina, CancellationToken cancellationToken)
    {
        IQueryable<SalaryHistory> query = _context.SalaryHistories
            .AsNoTracking().Where(x => x.Employee.IdKnowledge == idKnowledge);

        int totalItens = await query.CountAsync(cancellationToken);

        List<SalaryHistory> itens = await query
            .OrderBy(a => a.CreatedAt)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<SalaryHistory>.Create(
            itens,
            totalItens,
            pagina,
            tamanhoPagina
        );
    }

    public async Task<SalaryHistory?> SearchSalaryHistoryByIdAndIdKnowledge(Guid id, Guid idKnowledge, CancellationToken cancellationToken)
    {
        return await _context.SalaryHistories.FirstOrDefaultAsync(salary => salary.Id == id && salary.Employee.IdKnowledge == idKnowledge, cancellationToken);
    }

    public void EditSalaryHistory(SalaryHistory updatedSalaryHistory)
    {
        _context.SalaryHistories.Update(updatedSalaryHistory);
    }

    public async Task CreateSalaryHistory(SalaryHistory newSalaryHistory, CancellationToken cancellationToken)
    {
        await _context.SalaryHistories.AddAsync(newSalaryHistory, cancellationToken);
    }

    public async Task AddAsync(IList<SalaryHistory> salaryHistories, CancellationToken cancellationToken)
    {
        await _context.SalaryHistories.AddRangeAsync(salaryHistories, cancellationToken);
    }

    public void RemoveSalaryHistoryByIdAndIdKnowledge(SalaryHistory salaryHistory, CancellationToken cancellationToken)
    {
        _context.SalaryHistories.Remove(salaryHistory);
    }
}