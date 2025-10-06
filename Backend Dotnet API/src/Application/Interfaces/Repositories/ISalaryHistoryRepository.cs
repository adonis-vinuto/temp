using Application.Common.Responses;
using Application.Interfaces.Data;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISalaryHistoryRepository
{
    IUnitOfWork UnitOfWork { get; }

    public Task<PagedResponse<SalaryHistory>> SearchSalaryHistoryByIdKnowledge(Guid idKnowledge, int pagina, int tamanhoPagina, CancellationToken cancellationToken);

    public Task<SalaryHistory?> SearchSalaryHistoryByIdAndIdKnowledge(Guid id, Guid idKnowledge, CancellationToken cancellationToken);

    public void EditSalaryHistory(SalaryHistory updatedSalaryHistory);

    public Task CreateSalaryHistory(SalaryHistory newSalaryHistory, CancellationToken cancellationToken);

    public Task AddAsync(IList<SalaryHistory> salaryHistories, CancellationToken cancellationToken);

    public void RemoveSalaryHistoryByIdAndIdKnowledge(SalaryHistory salaryHistory, CancellationToken cancellationToken);
}
