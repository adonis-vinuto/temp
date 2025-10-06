using Application.Handlers;
using ErrorOr;
using MapsterMapper;
using Application.DTOs.Knowledge;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;

namespace Application.Handlers.Knowledge.SearchById;

public class SearchKnowledgeByIdHandler : BaseHandler
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public SearchKnowledgeByIdHandler(IKnowledgeRepository knowledgeRepository, IMapper mapper, IAuthenticationService authenticationService)
    {
        _knowledgeRepository = knowledgeRepository;
        _mapper = mapper;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<KnowledgeResponse>> Handle(SearchKnowledgeByIdRequest request, Module module, CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchKnowledgeByIdRequestValidator()) is var resultado && resultado.Any())
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

        return _mapper.Map<KnowledgeResponse>(knowledge);
    }
}