using Application.DTOs.Knowledge;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.Knowledge.Edit;

public class EditKnowledgeHandler : BaseHandler
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;

    public EditKnowledgeHandler(IKnowledgeRepository knowledgeRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService)
    {
        _knowledgeRepository = knowledgeRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
    }

    public async Task<ErrorOr<KnowledgeResponse>> Handle(EditKnowledgeRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new EditKnowledgeRequestValidator()) is var resultado && resultado.Any())
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

        Domain.Entities.Knowledge? knowledge = await _knowledgeRepository.SearchByIdAsync(request.Id, module, cancellationToken);

        if (knowledge is null)
        {
            return KnowledgeErrors.KnowledgeNotFound;
        }

        knowledge = request.Adapt(knowledge);

        _knowledgeRepository.Update(knowledge);
        await _knowledgeRepository.UnitOfWork.Commit();

        KnowledgeResponse knowledgeResponse = knowledge.Adapt<KnowledgeResponse>();
        return knowledgeResponse;
    }
}