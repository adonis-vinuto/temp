using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Handlers.People.Payroll.Remove;

public sealed record RemovePayrollRequest(Guid IdPayroll, Guid IdKnowledge );
public class RemovePayrollRequestValidator : AbstractValidator<RemovePayrollRequest>
{
    public RemovePayrollRequestValidator()
    {
        RuleFor(x => x.IdPayroll)
            .NotNull()
            .WithMessage("IdPayroll não deve ser nulo.")
            .NotEmpty()
            .WithMessage("IdPayroll não pode ser vazio.");
        
        RuleFor(x => x.IdKnowledge)
            .NotNull()
            .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID não pode ser vazio.");
        
    }
}
