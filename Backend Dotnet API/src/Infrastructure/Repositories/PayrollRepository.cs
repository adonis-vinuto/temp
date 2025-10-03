using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Common.Responses;
using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PayrollRepository : IPayrollRepository
{
    public IUnitOfWork UnitOfWork => _context;

    private readonly TenantDbContext _context;

    public PayrollRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payroll payroll, CancellationToken cancellationToken)
    {
        await _context.Payrolls.AddAsync(payroll, cancellationToken);
    }

    public async Task AddAsync(IList<Payroll> payrolls, CancellationToken cancellationToken)
    {
        await _context.Payrolls.AddRangeAsync(payrolls, cancellationToken);
    }

    public async Task<Payroll?> SearchByIdAsync(Guid idPayroll, CancellationToken cancellationToken)
    {
        return await (from p in _context.Payrolls where p.Id == idPayroll select p)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Payroll?> SearchByIdAsync(Guid idPayroll, Guid idKnowledge, CancellationToken cancellationToken)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Include(p => p.Employee)
            .Where(p => p.Id == idPayroll && p.Employee.IdKnowledge == idKnowledge)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Edit(Payroll payroll)
    {
        _context.Payrolls.Update(payroll);
    }

    public void Remove(Payroll payroll)
    {
        _context.Payrolls.Remove(payroll);
    }

    public async Task<PagedResponse<Payroll>> PagedSearchAsync(Guid idKnowledge, int pagina, int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<Payroll> query =
            _context.Payrolls
                .Include(p => p.Employee)
                .AsNoTracking().Where(x => x.Employee.IdKnowledge == idKnowledge);

        int total = await query.CountAsync(cancellationToken);

        List<Payroll> itens = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<Payroll>.Create(itens, total, pagina, tamanhoPagina);
    }
}

