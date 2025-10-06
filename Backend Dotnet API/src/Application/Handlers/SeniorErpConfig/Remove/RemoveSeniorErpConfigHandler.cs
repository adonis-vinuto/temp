using Application.DTOs.Agent;
using Application.DTOs.SeniorErpConfig;
using Application.Handlers.Agent.Remove;
using Application.Interfaces.Repositories;
using Domain.Entities;
using ErrorOr;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.SeniorErpConfig.Remove;

public class RemoveSeniorErpConfigHandler : BaseHandler
{
    private readonly ISeniorErpConfigRepository _seniorErpConfigRepository;

    public RemoveSeniorErpConfigHandler(ISeniorErpConfigRepository seniorErpConfigRepository)
    {
        _seniorErpConfigRepository = seniorErpConfigRepository;
    }

    public async Task<ErrorOr<SeniorErpConfigResponse>> Handle(RemoveSeniorErpConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new RemoveSeniorErpConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }   

        Domain.Entities.SeniorErpConfig? seniorErp = await _seniorErpConfigRepository.SearchByIdAsync(request.Id, cancellationToken);

        if (seniorErp is null)
        {
            return Domain.Errors.SeniorErpConfigErrors.NotFound;
        }

        _seniorErpConfigRepository.Remove(seniorErp);
        await _seniorErpConfigRepository.UnitOfWork.Commit();

        SeniorErpConfigResponse seniorErpConfigResponse = seniorErp.Adapt<SeniorErpConfigResponse>();
        return seniorErpConfigResponse;
    }
}
