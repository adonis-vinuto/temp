using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IChatSessionRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<ChatSession?> GetByIdAsync(Guid id, string idUser, CancellationToken cancellationToken);
    Task<ChatSession?> GetByIdWithHistoryAsync(Guid id, string idUser, CancellationToken cancellationToken);
    Task<ChatSession?> GetByAgentAndUserAsync(Guid idAgent, string idUser, Module module, CancellationToken cancellationToken);
    Task AddAsync(ChatSession chatSession, CancellationToken cancellationToken);
    void Update(ChatSession chatSession);
}