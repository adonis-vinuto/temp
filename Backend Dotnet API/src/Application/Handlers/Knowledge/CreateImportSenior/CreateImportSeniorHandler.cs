using Application.DTOs.Knowledge;
using Application.DTOs.Knowledge.ImportSenior;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;
using System.Transactions;
using System.Xml.Linq;

namespace Application.Handlers.Knowledge.CreateImportSenior;

public class CreateImportSeniorHandler : BaseHandler
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly ISeniorHcmConfigRepository _hcmConfigRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly ISeniorService _seniorService;

    public CreateImportSeniorHandler(IKnowledgeRepository knowledgeRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        ISeniorHcmConfigRepository hcmConfigRepository,
        ISeniorService seniorService)
    {
        _knowledgeRepository = knowledgeRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _hcmConfigRepository = hcmConfigRepository;
        _seniorService = seniorService;
    }

    public async Task<ErrorOr<KnowledgeResponse>> Handle(CreateImportSeniorRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateKnowledgeRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (!_moduleService.HasAccessToModule(user, module))
        {
            return UserErrors.ModuleAccessDenied;
        }

        Domain.Entities.Knowledge? knowledge = await _knowledgeRepository.SearchByIdAsync(request.IdKnowledge, module, cancellationToken);

        if (knowledge is null)
        {
            return KnowledgeErrors.KnowledgeNotFound;
        }

        Domain.Entities.SeniorHcmConfig? seniorHcmConfig = await _hcmConfigRepository.SearchByIdAsync(request.IdSeniorHcmConfig, cancellationToken);

        if (seniorHcmConfig is null)
        {
            return SeniorHcmConfigErrors.NotFound;
        }

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        XDocument response = await _seniorService.SendRequest(new Senior(seniorHcmConfig.WsdlUrl, seniorHcmConfig.Username, seniorHcmConfig.Password, "biPessoas", "", "10/06/2025"));
        await _seniorService.SalvaDadosEmployee(response, request.IdKnowledge);

        response = await _seniorService.SendRequest(new Senior(seniorHcmConfig.WsdlUrl, seniorHcmConfig.Username, seniorHcmConfig.Password, "biHsa", "", "10/06/2025"));
        await _seniorService.SalvaDadosSalaryHistory(response, request.IdKnowledge);

        response = await _seniorService.SendRequest(new Senior(seniorHcmConfig.WsdlUrl, seniorHcmConfig.Username, seniorHcmConfig.Password, "biFichaFinan", "", "01/01/2025"));
        await _seniorService.SalvaDadosPayroll(response, request.IdKnowledge);

        scope.Complete();

        KnowledgeResponse knowledgeResponse = knowledge.Adapt<KnowledgeResponse>();
        return knowledgeResponse;
    }
}