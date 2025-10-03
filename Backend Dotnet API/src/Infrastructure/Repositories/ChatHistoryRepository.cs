using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatHistoryRepository : IChatHistoryRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public ChatHistoryRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<ChatHistory?> SearchChatHistoryByIdSession(
        Guid idSession,
        string idUser,
        Module module,
        CancellationToken cancellationToken)
    {
        return await _context.ChatsHistory
            .Include(x => x.ChatSession)
            .ThenInclude(x => x.Agent)
            .FirstOrDefaultAsync(
                x => x.IdChatSession == idSession &&
                x.ChatSession.IdUser == idUser &&
                x.ChatSession.Agent.Module == module,
                cancellationToken
            );
    }

    public async Task<List<ChatHistory>> GetHistoryBySessionAsync(
        Guid idSession,
        string idUser,
        Module module,
        CancellationToken cancellationToken)
    {
        return await _context.ChatsHistory
            .Where(x =>
                x.IdChatSession == idSession &&
                x.ChatSession.IdUser == idUser &&
                x.ChatSession.Agent.Module == module)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        await _context.ChatsHistory.AddAsync(chatHistory, cancellationToken);
    }
}