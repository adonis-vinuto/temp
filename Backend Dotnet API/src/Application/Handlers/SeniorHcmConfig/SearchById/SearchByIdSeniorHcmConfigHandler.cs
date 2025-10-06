using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.SeniorHcmConfig;
using Application.Interfaces.Repositories;
using ErrorOr;
using Mapster;

namespace Application.Handlers.SeniorHcmConfig.SearchById;

public class SearchByIdSeniorHcmConfigHandler : BaseHandler
{
    private readonly ISeniorHcmConfigRepository _seniorHcmConfigRepository;

    public SearchByIdSeniorHcmConfigHandler(ISeniorHcmConfigRepository seniorHcmConfigRepository)
    {
        _seniorHcmConfigRepository = seniorHcmConfigRepository;
    }

    public async Task<ErrorOr<SeniorHcmConfigResponse>> Handle(SearchByIdSeniorHcmConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new SearchByIdSeniorHcmConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        Domain.Entities.SeniorHcmConfig? seniorHcm = await _seniorHcmConfigRepository.SearchByIdAsync(request.Id, cancellationToken);

        if (seniorHcm is null)
        {
            return Domain.Errors.SeniorErpConfigErrors.NotFound;
        }

        SeniorHcmConfigResponse seniorErpConfigResponse = seniorHcm.Adapt<SeniorHcmConfigResponse>();
        return seniorErpConfigResponse;
    }
}
