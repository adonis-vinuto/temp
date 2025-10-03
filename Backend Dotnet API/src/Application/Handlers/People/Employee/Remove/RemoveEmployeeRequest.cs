using FluentValidation;

namespace Application.Handlers.People.Employee.Remove;

public sealed record RemoveEmployeeRequest(Guid IdKnowledge, string IdEmployee);

public class RemoveEmployeeRequestValidator : AbstractValidator<RemoveEmployeeRequest>
{
    public RemoveEmployeeRequestValidator()
    {
        RuleFor(x => x.IdKnowledge)
            .NotNull()
            .WithMessage("IdKnowledge não deve ser nulo.")
            .NotEmpty()
            .WithMessage("IdKnowledge não pode ser vazio.");

        RuleFor(x => x.IdEmployee)
            .NotNull()
            .WithMessage("IdEmployee não deve ser nulo.")
            .NotEmpty()
            .WithMessage("IdEmployee não pode ser vazio.");
    }
}