using Application.Handlers;
using ErrorOr;
using MapsterMapper;
using Application.Interfaces.Repositories;
using Domain.Errors;
using Application.DTOs.DataConfig;
using Application.Interfaces.Services;
using Authentication.Models;

namespace Application.Handlers.DataConfig.SearchById;

public class SearchDataConfigByIdHandler : BaseHandler
{
    private readonly IDataConfigRepository _dataConfigRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public SearchDataConfigByIdHandler(IDataConfigRepository dataConfigRepository, IAuthenticationService authenticationService,
        IMapper mapper)
    {
        _dataConfigRepository = dataConfigRepository;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<ErrorOr<DataConfigResponse>> Handle(SearchDataConfigByIdRequest request, CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchDataConfigByIdRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }
        
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        
        Domain.Entities.DataConfig? dataConfig = await _dataConfigRepository.SearchByIdAsync(request.Id, user.Organizations.FirstOrDefault()!, cancellationToken);

        if (dataConfig is null)
        {
            return DataConfigErrors.DataConfigNotFound;
        }

        return _mapper.Map<DataConfigResponse>(dataConfig);
    }
}