using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Handlers.People.Payroll.SearchById;

public sealed record SearchByIdPayrollRequest(Guid Id, Guid IdKnowledge);
public class SearchByIdPayrollRequestValidator : AbstractValidator<SearchByIdPayrollRequest>
{
    public SearchByIdPayrollRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("IdPayroll é necessário.")
            .NotNull().WithMessage("IdPayroll não pode ser nulo.");

        RuleFor(x => x.IdKnowledge)
            .NotEmpty().WithMessage("IdKnowledge é necessário.")
            .NotNull().WithMessage("IdKnowledge não pode ser nulo.");
    }
}
