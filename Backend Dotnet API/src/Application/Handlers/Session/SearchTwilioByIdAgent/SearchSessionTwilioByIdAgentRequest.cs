using FluentValidation;

namespace Application.Handlers.Session.SearchTwilioByIdAgent;

public record SearchSessionTwilioByIdAgentRequest(Guid IdAgent);

public class SearchSessionTwilioByIdAgentRequestValidator : AbstractValidator<SearchSessionTwilioByIdAgentRequest>
{
    public SearchSessionTwilioByIdAgentRequestValidator()
    {
        RuleFor(x => x.IdAgent)
            .NotNull()
            .WithMessage("ID do Agente não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID do Agente não pode ser vazio.");
    }
}