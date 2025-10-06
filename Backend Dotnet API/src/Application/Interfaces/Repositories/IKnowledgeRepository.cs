
using Application.Common.Responses;
using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IKnowledgeRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(Knowledge knowledge, CancellationToken cancellationToken);

    Task<Knowledge?> SearchByIdAsync(Guid idKnowledge, Module module, CancellationToken cancellationToken);

    Task<PagedResponse<Knowledge>> PagedSearchAsync(
        Module module,
        Guid? idAgent,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken
    );

    public void Update(Knowledge knowledge);

    public void Remove(Knowledge knowledge);
}