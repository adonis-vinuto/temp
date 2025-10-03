using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.SeniorHcmConfig;
using Application.Interfaces.Repositories;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.SeniorHcmConfig.Edit;

public class EditSeniorHcmConfigHandler : BaseHandler
{
    private readonly ISeniorHcmConfigRepository _seniorHcmConfigRepository;

    public EditSeniorHcmConfigHandler(ISeniorHcmConfigRepository seniorHcmConfigRepository)
    {
        _seniorHcmConfigRepository = seniorHcmConfigRepository;
    }

    public async Task<ErrorOr<SeniorHcmConfigResponse>> Handle(EditSeniorHcmConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new EditSeniorHcmConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        Domain.Entities.SeniorHcmConfig? seniorHcmConfig = await _seniorHcmConfigRepository.SearchByIdAsync(request.Id, cancellationToken);
        
        if (seniorHcmConfig is null)
        {
            return SeniorHcmConfigErrors.NotFound;
        }

        seniorHcmConfig.Username = request.Username ?? seniorHcmConfig.Username;
        seniorHcmConfig.Password = request.Password ?? seniorHcmConfig.Password;
        seniorHcmConfig.WsdlUrl = request.WsdlUrl?.ToString() ?? seniorHcmConfig.WsdlUrl;

        _seniorHcmConfigRepository.Edit(seniorHcmConfig);
        await _seniorHcmConfigRepository.UnitOfWork.Commit();

        SeniorHcmConfigResponse response = seniorHcmConfig.Adapt<SeniorHcmConfigResponse>();
        return response;
    }
}
