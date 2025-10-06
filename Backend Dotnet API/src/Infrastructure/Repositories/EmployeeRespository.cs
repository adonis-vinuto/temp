using Application.Common.Responses;
using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public EmployeeRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
    }

    public async Task AddAsync(IList<Employee> employees, CancellationToken cancellationToken)
    {
        await _context.Employees.AddRangeAsync(employees, cancellationToken);
    }

    public async Task<Employee?> SearchByIdAsync(Guid idKnowledge, string idEmployee, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => idKnowledge == e.IdKnowledge && e.Id == idEmployee, cancellationToken);
    }

    public async Task<Employee?> SearchByIdEmployeeAsync(string idEmployee, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .Include(x => x.SalaryHistories)
            .Include(x => x.Payrolls)
            .FirstOrDefaultAsync(e => e.Id == idEmployee, cancellationToken);
    }

    public void Edit(Employee employee)
    {
        _context.Employees.Update(employee);
    }

    public void Remove(Employee employee)
    {
        _context.SalaryHistories.RemoveRange(employee.SalaryHistories);
        _context.Payrolls.RemoveRange(employee.Payrolls);
        _context.Employees.Remove(employee);
    }

    public async Task<PagedResponse<Employee>> PagedSearchAsync(
        Guid idKnowledge,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<Employee> query = _context.Employees
            .AsNoTracking().Where(x => x.IdKnowledge == idKnowledge);

        int totalItens = await query.CountAsync(cancellationToken);

        List<Employee> itens = await query
            .OrderBy(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return PagedResponse<Employee>.Create(
            itens,
            totalItens,
            pagina,
            tamanhoPagina
        );
    }
}