using Application.Common.Responses;
using Application.Interfaces.Data;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISeniorErpConfigRepository
{
    IUnitOfWork UnitOfWork { get; }
    
    Task AddAsync(SeniorErpConfig config, CancellationToken cancellationToken);

    Task<SeniorErpConfig?> SearchByIdAsync(Guid idConfig, CancellationToken cancellationToken);

    public void Edit(SeniorErpConfig config);

    Task<PagedResponse<SeniorErpConfig>> PagedSearchAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken
    );

    public void Remove(SeniorErpConfig config);
    
    public void Update(SeniorErpConfig seniorErpConfig);
}
