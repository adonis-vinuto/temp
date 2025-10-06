using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatSessionRepository : IChatSessionRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public ChatSessionRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<ChatSession?> GetByIdAsync(Guid id, string idUser, CancellationToken cancellationToken)
    {
        return await _context.Chats
            .FirstOrDefaultAsync(x => x.Id == id && x.IdUser == idUser, cancellationToken);
    }

    public async Task<ChatSession?> GetByIdWithHistoryAsync(Guid id, string idUser, CancellationToken cancellationToken)
    {
        return await _context.Chats
            .Include(x => x.ChatHistory)
            .Include(x => x.Agent)
            .FirstOrDefaultAsync(x => x.Id == id && x.IdUser == idUser, cancellationToken);
    }

    public async Task<ChatSession?> GetByAgentAndUserAsync(
        Guid idAgent,
        string idUser,
        Module module,
        CancellationToken cancellationToken)
    {
        return await _context.Chats
            .Include(x => x.Agent)
            .FirstOrDefaultAsync(
                x => x.IdAgent == idAgent &&
                     x.IdUser == idUser &&
                     x.Agent.Module == module,
                cancellationToken
            );
    }

    public async Task AddAsync(ChatSession chatSession, CancellationToken cancellationToken)
    {
        await _context.Chats.AddAsync(chatSession, cancellationToken);
    }

    public void Update(ChatSession chatSession)
    {
        _context.Chats.Update(chatSession);
    }
}