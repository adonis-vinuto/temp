using Application.Interfaces.Data;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IChatHistoryRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<ChatHistory?> SearchChatHistoryByIdSession(Guid idSession, string idUser, Module module, CancellationToken cancellationToken);
    Task<List<ChatHistory>> GetHistoryBySessionAsync(Guid idSession, string idUser, Module module, CancellationToken cancellationToken);
    Task AddAsync(ChatHistory chatHistory, CancellationToken cancellationToken);
}