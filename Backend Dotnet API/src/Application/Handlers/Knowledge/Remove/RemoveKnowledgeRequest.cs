using FluentValidation;

namespace Application.Handlers.Knowledge.Remove;

public sealed record RemoveKnowledgeRequest(Guid Id);

public class RemoveKnowledgeRequestValidator : AbstractValidator<RemoveKnowledgeRequest>
{
    public RemoveKnowledgeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID não pode ser vazio.");
    }
}