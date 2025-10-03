using Application.DTOs.Knowledge;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.Knowledge.Create;

public class CreateKnowledgeHandler : BaseHandler
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;

    public CreateKnowledgeHandler(IKnowledgeRepository knowledgeRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService)
    {
        _knowledgeRepository = knowledgeRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
    }

    public async Task<ErrorOr<KnowledgeResponse>> Handle(CreateKnowledgeRequest request, Module module, CancellationToken cancellationToken)
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

        var knowledge = new Domain.Entities.Knowledge
        {
            Name = request.Name,
            Description = request.Description,
            Origin = request.Origin,
            Module = module
        };

        await _knowledgeRepository.AddAsync(knowledge, cancellationToken);
        await _knowledgeRepository.UnitOfWork.Commit();

        KnowledgeResponse knowledgeResponse = knowledge.Adapt<KnowledgeResponse>();
        return knowledgeResponse;
    }
}