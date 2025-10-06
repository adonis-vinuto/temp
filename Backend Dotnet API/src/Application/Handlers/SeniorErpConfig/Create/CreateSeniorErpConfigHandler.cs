using Application.Common.Responses;
using Application.DTOs.SeniorErpConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.SeniorErpConfig.Create;

public class CreateSeniorErpConfigHandler : BaseHandler
{
    private readonly ISeniorErpConfigRepository _seniorErpConfigRepository;
    private readonly IAuthenticationService _authenticationService;

    public CreateSeniorErpConfigHandler(
        ISeniorErpConfigRepository seniorErpConfigRepository,
        IAuthenticationService authenticationService)
    {
        _seniorErpConfigRepository = seniorErpConfigRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<SeniorErpConfigResponse>> Handle(CreateSeniorErpConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateSeniorErpConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        var seniorConfig = new Domain.Entities.SeniorErpConfig
        {
            Username = request.Username,
            Password = request.Password,
            WsdlUrl = request.WsdlUrl,
        };
        
        await _seniorErpConfigRepository.AddAsync(seniorConfig, cancellationToken);
        await _seniorErpConfigRepository.UnitOfWork.Commit();
        
        SeniorErpConfigResponse response = seniorConfig.Adapt<SeniorErpConfigResponse>();
        return response;
        
    }
}
