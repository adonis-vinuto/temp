
using Application.Common.Responses;
using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IDataConfigRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(DataConfig dataConfig, CancellationToken cancellationToken);

    Task<DataConfig?> SearchByIdAsync(Guid idDataConfig, string organization, CancellationToken cancellationToken);

    Task<DataConfig?> GetDataConfigByOrganization(string organization, CancellationToken cancellationToken);
    
    Task<PagedResponse<DataConfig>> PagedSearchAsync(
        string organization,
        Module? module,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken
    );

    public void Edit(DataConfig dataConfig);

    Task<string?> GetTenantConnectionStringAsync(string organizationName);
}