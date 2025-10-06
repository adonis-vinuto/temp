using Application.Common.Responses;
using Application.Interfaces.Data;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using File = Domain.Entities.File;

namespace Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    public IUnitOfWork UnitOfWork => _context;
    private readonly TenantDbContext _context;

    public FileRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(File file, CancellationToken cancellationToken)
    {
        await _context.Files.AddAsync(file, cancellationToken);
    }

    public async Task<File?> SearchByIdAsync(Guid idFile, Module module, CancellationToken cancellationToken)
    {
        return await _context.Files
            .Include(x => x.Agents)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == idFile, cancellationToken
            );
    }

    public void Update(File file)
    {
        File? existingFile = _context.Files
                .Include(f => f.Agents)
                .FirstOrDefault(f => f.Id == file.Id);

        if (existingFile != null)
        {
            _context.Entry(existingFile).CurrentValues.SetValues(file);

            SyncAgents(existingFile, file);
        }
        else
        {
            _context.Files.Update(file);
        }
    }

    private void SyncAgents(File target, File source)
    {
        HashSet<Guid> targetAgentIds = target.Agents?.Select(a => a.Id).ToHashSet() ?? [];
        HashSet<Guid> sourceAgentIds = source.Agents?.Select(a => a.Id).ToHashSet() ?? [];

        foreach (Agent? agent in target.Agents?.Where(a => !sourceAgentIds.Contains(a.Id)).ToList() ?? [])
        {
            target.RemoveAgent(agent.Id);
        }

        foreach (Agent? agent in source.Agents?.Where(a => !targetAgentIds.Contains(a.Id)).ToList() ?? [])
        {
            Agent trackedAgent = _context.Agents.Local.FirstOrDefault(a => a.Id == agent.Id)
                                 ?? _context.Agents.Find(agent.Id)
                                 ?? agent;

            target.AddAgent(trackedAgent);
        }
    }



    public void Remove(File file)
    {
        _context.Files.Remove(file);
    }

    public async Task<PagedResponse<File>> PagedSearchAsync(
        Module module,
        Guid? idAgent,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        IQueryable<File> query = _context.Files
            .Include(x => x.Agents)
            .AsNoTracking().Where(x => x.Module == module);

        if (idAgent != null)
        {
            query = query.Where(x => x.Agents != null && x.Agents.Any(x => x.Id == idAgent));
        }

        int totalItens = await query.CountAsync(cancellationToken);

        List<File> itens = await query
            .OrderBy(a => a.CreatedAt)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return PagedResponse<File>.Create(
            itens,
            totalItens,
            pagina,
            tamanhoPagina
        );
    }
}
