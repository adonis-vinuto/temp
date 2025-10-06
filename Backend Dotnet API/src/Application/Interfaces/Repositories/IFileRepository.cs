
using Application.Common.Responses;
using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IFileRepository
{
    IUnitOfWork UnitOfWork { get; }

    Task AddAsync(Domain.Entities.File file, CancellationToken cancellationToken);

    Task<Domain.Entities.File?> SearchByIdAsync(Guid idFile, Module module, CancellationToken cancellationToken);

    Task<PagedResponse<Domain.Entities.File>> PagedSearchAsync(
        Module module,
        Guid? idAgent,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken
    );

    public void Update(Domain.Entities.File file);

    public void Remove(Domain.Entities.File file);
}