using FluentValidation;

namespace Application.Handlers.Agent.Remove;

public sealed record RemoveAgentRequest(Guid Id);

public class RemoveAgentRequestValidator : AbstractValidator<RemoveAgentRequest>
{
    public RemoveAgentRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID não pode ser vazio.");
    }
}