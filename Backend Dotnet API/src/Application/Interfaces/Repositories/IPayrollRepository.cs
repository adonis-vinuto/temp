using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Responses;
using Application.Interfaces.Data;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPayrollRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(Payroll payroll, CancellationToken cancellationToken);

    Task AddAsync(IList<Payroll> payrolls, CancellationToken cancellationToken);

    Task<Payroll?> SearchByIdAsync(Guid idPayroll, Guid idKnowledge, CancellationToken cancellationToken);

    void Edit(Payroll payroll);

    Task<PagedResponse<Payroll>> PagedSearchAsync(
        Guid idKnowledge,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken);

    void Remove(Payroll payroll);
}
