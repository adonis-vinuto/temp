using FluentValidation;

namespace Application.Handlers.People.SalaryHistory.Remove;

public sealed record RemoveSalaryHistoryRequest(Guid IdSalaryHistory, Guid IdKnowledge);

public class RemoveSalaryHistoryRequestValidator : AbstractValidator<RemoveSalaryHistoryRequest>
{
    public RemoveSalaryHistoryRequestValidator()
    {
        RuleFor(x => x.IdSalaryHistory)
            .NotNull()
            .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID não pode ser vazio.");

        RuleFor(x => x.IdKnowledge)
            .NotNull()
            .WithMessage("ID Knowledge não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID Knowledge não pode ser vazio.");
    }
}