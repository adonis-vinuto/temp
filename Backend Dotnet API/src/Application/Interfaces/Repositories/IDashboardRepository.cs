using Application.DTOs.Dashboard;
using Application.Interfaces.Data;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IDashboardRepository
{
    IUnitOfWork UnitOfWork { get; }
    
    Task<DashboardResponse> GetDashboardAsync(Module module, CancellationToken cancellationToken);
}