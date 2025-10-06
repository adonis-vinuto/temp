using Application.DTOs.SeniorErpConfig;
using Application.Interfaces.Repositories;
using ErrorOr;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.SeniorErpConfig.SearchById;

public class SearchByIdSeniorErpConfigHandler : BaseHandler
{
    private readonly ISeniorErpConfigRepository _seniorErpConfigRepository;
    
    public SearchByIdSeniorErpConfigHandler(ISeniorErpConfigRepository seniorErpConfigRepository)
    {
        _seniorErpConfigRepository = seniorErpConfigRepository;
    }

    public async Task<ErrorOr<SeniorErpConfigResponse>> Handle(SearchByIdSeniorErpConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new SearchByIdSeniorErpConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        Domain.Entities.SeniorErpConfig? seniorErp = await _seniorErpConfigRepository.SearchByIdAsync(request.Id, cancellationToken);

        if (seniorErp is null)
        {
            return Domain.Errors.SeniorErpConfigErrors.NotFound;
        }


        SeniorErpConfigResponse seniorErpConfigResponse = seniorErp.Adapt<SeniorErpConfigResponse>();

        return seniorErpConfigResponse;
    }
}
