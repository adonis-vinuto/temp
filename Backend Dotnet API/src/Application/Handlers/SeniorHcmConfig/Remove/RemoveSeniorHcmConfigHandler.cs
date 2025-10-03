using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.SeniorHcmConfig;
using Application.Interfaces.Repositories;
using ErrorOr;
using Mapster;

namespace Application.Handlers.SeniorHcmConfig.Remove;

public class RemoveSeniorHcmConfigHandler : BaseHandler
{
     private readonly ISeniorHcmConfigRepository _seniorHcmConfigRepository;

    public RemoveSeniorHcmConfigHandler(ISeniorHcmConfigRepository seniorHcmConfigRepository)
    {
        _seniorHcmConfigRepository = seniorHcmConfigRepository;
    }

    public async Task<ErrorOr<SeniorHcmConfigResponse>> Handle(RemoveSeniorHcmConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new RemoveSeniorHcmConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }   

        Domain.Entities.SeniorHcmConfig? seniorHcm = await _seniorHcmConfigRepository.SearchByIdAsync(request.Id, cancellationToken);

        if (seniorHcm is null)
        {
            return Domain.Errors.SeniorHcmConfigErrors.NotFound;
        }

        _seniorHcmConfigRepository.Remove(seniorHcm);
        await _seniorHcmConfigRepository.UnitOfWork.Commit();

        SeniorHcmConfigResponse seniorHcmConfigResponse = seniorHcm.Adapt<SeniorHcmConfigResponse>();
        return seniorHcmConfigResponse;
    }
}