using Application.DTOs.Knowledge;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.Knowledge.Remove;

public sealed class RemoveKnowledgeHandler : BaseHandler
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IAuthenticationService _authenticationService;

    public RemoveKnowledgeHandler(IKnowledgeRepository knowledgeRepository,
        IAuthenticationService authenticationService)
    {
        _knowledgeRepository = knowledgeRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<KnowledgeResponse>> Handle(RemoveKnowledgeRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new RemoveKnowledgeRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.Knowledge? knowledge = await _knowledgeRepository.SearchByIdAsync(request.Id, module, cancellationToken);

        if (knowledge is null)
        {
            return KnowledgeErrors.KnowledgeNotFound;
        }

        _knowledgeRepository.Remove(knowledge);
        await _knowledgeRepository.UnitOfWork.Commit();

        KnowledgeResponse knowledgeResponse = knowledge.Adapt<KnowledgeResponse>();
        return knowledgeResponse;
    }
}