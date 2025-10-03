using FluentValidation;

namespace Application.Handlers.People.Employee.SearchById;

public sealed record SearchEmployeeByIdRequest(Guid IdKnowledge, string IdEmployee);

public class SearchEmployeeByIdRequestValidator : AbstractValidator<SearchEmployeeByIdRequest>
{
    public SearchEmployeeByIdRequestValidator()
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