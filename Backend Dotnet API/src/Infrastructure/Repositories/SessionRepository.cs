using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public SessionRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChatSession>> SearchChatSessionByIdAgent(
        Guid idAgent,
        string idUser,
        Module module,
        CancellationToken cancellationToken)
    {
        return await _context.Chats
            .Include(x => x.Agent)
            .Where(
                x => x.IdAgent == idAgent &&
                     x.IdUser == idUser &&
                     x.Agent.Module == module)
            .ToListAsync(cancellationToken);
    }

    public async Task<ChatSession> SearchChatSessionTwilioByIdAgent(
        Guid idAgent,
        Module module,
        CancellationToken cancellationToken)
    {
        return await _context.Chats
            .Include(x => x.Agent)
            .FirstOrDefaultAsync(
                x => x.IdAgent == idAgent &&
                    x.Agent.Module == module, cancellationToken
            );
    }
}
