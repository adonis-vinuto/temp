using Application.Common.Responses;
using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface ITwilioConfigRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(TwilioConfig twilioConfig, CancellationToken cancellationToken);

    Task<TwilioConfig?> SearchByIdAsync(Guid id, Module module, CancellationToken cancellationToken);

    Task<TwilioConfig?> SearchByAgentIdAsync(Guid idAgent, Module module, CancellationToken cancellationToken);

    Task<PagedResponse<TwilioConfig>> PagedSearchAsync(
        Module module,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken);

    void Edit(TwilioConfig twilioConfig);

    void Remove(TwilioConfig twilioConfig);
}
