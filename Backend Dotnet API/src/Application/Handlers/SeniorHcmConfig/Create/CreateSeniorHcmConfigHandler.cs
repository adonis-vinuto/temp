using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.SeniorHcmConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.SeniorHcmConfig.Create;

public class CreateSeniorHcmConfigHandler : BaseHandler
{
    private readonly ISeniorHcmConfigRepository _seniorHcmConfigRepository;
    private readonly IAuthenticationService _authenticationService;

    public CreateSeniorHcmConfigHandler(
        ISeniorHcmConfigRepository seniorHcmConfigRepository,
        IAuthenticationService authenticationService)
    {
        _seniorHcmConfigRepository = seniorHcmConfigRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<SeniorHcmConfigResponse>> Handle(
        CreateSeniorHcmConfigRequest request,
        CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateSeniorHcmConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        var seniorConfig = new Domain.Entities.SeniorHcmConfig
        {
            Username = request.Username,
            Password = request.Password,
            WsdlUrl = request.WsdlUrl
        };

        await _seniorHcmConfigRepository.AddAsync(seniorConfig, cancellationToken);
        await _seniorHcmConfigRepository.UnitOfWork.Commit();

        SeniorHcmConfigResponse response = seniorConfig.Adapt<SeniorHcmConfigResponse>();
        return response;
    }
}
