using Application.DTOs.SeniorErpConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Errors;
using ErrorOr;
using Mapster;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Handlers.SeniorErpConfig.Edit;

public class EditSeniorErpConfigHandler : BaseHandler
{
    private readonly ISeniorErpConfigRepository _seniorErpConfigRepository;

    public EditSeniorErpConfigHandler(ISeniorErpConfigRepository seniorErpConfigRepository)
    {
        _seniorErpConfigRepository = seniorErpConfigRepository;    
    }

    public async Task<ErrorOr<SeniorErpConfigResponse>> Handle(EditSeniorErpConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new EditSeniorErpConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }
        
        Domain.Entities.SeniorErpConfig? seniorErpConfig = await _seniorErpConfigRepository.SearchByIdAsync(request.Id, cancellationToken);

        if (seniorErpConfig is null)
        {
            return SeniorErpConfigErrors.NotFound;
        }

        seniorErpConfig.Username = request.Username ?? seniorErpConfig.Username;
        seniorErpConfig.Password = request.Password ?? seniorErpConfig.Password;
        seniorErpConfig.WsdlUrl = request.WsdlUrl?.ToString() ?? seniorErpConfig.WsdlUrl;

        _seniorErpConfigRepository.Edit(seniorErpConfig);
        await _seniorErpConfigRepository.UnitOfWork.Commit();

        SeniorErpConfigResponse response = seniorErpConfig.Adapt<SeniorErpConfigResponse>();
        return response;
    }
}