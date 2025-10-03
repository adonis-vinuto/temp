using FluentValidation;

namespace Application.Handlers.Session.SearchByIdAgent;

public record SearchSessionByIdAgentRequest(Guid IdAgent, Guid IdUser);

public class SearchSessionByIdAgentRequestValidator : AbstractValidator<SearchSessionByIdAgentRequest>
{
    public SearchSessionByIdAgentRequestValidator()
    {
        RuleFor(x => x.IdAgent)
            .NotNull()
            .WithMessage("ID do Agente não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID do Agente não pode ser vazio.");

        RuleFor(x => x.IdUser)
            .NotNull()
            .WithMessage("ID do Usuário não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID do Usuário não pode ser vazio.");
    }
}