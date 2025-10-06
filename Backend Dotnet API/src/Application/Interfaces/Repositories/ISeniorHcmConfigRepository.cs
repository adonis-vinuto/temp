using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Responses;
using Application.Interfaces.Data;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISeniorHcmConfigRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(SeniorHcmConfig config, CancellationToken cancellationToken);

    Task<SeniorHcmConfig?> SearchByIdAsync(Guid idConfig, CancellationToken cancellationToken);

    public void Edit(SeniorHcmConfig config);

    Task<PagedResponse<SeniorHcmConfig>> PagedSearchAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken
    );

    public void Remove(SeniorHcmConfig config);
    
    public void Update(SeniorHcmConfig seniorHcmConfig);
}