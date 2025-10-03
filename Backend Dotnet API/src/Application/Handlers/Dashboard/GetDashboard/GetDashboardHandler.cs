using ErrorOr;
using MapsterMapper;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using Application.DTOs.Dashboard;

namespace Application.Handlers.Dashboard.GetDashboard;

public class GetDashboardHandler : BaseHandler
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public GetDashboardHandler(
        IDashboardRepository dashboardRepository, 
        IMapper mapper, 
        IAuthenticationService authenticationService)
    {
        _dashboardRepository = dashboardRepository;
        _mapper = mapper;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<DashboardResponse>> Handle(
        Module module,
        CancellationToken cancellationToken = default)
    {
        
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();
        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        DashboardResponse dashboard = await _dashboardRepository.GetDashboardAsync(
            module,
            cancellationToken);
            
        if (dashboard is null)
        {
            return DashboardErrors.NotFound;
        }

        return _mapper.Map<DashboardResponse>(dashboard);
    }
}
