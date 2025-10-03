using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.SeniorErpConfig.Remove;

public record RemoveSeniorErpConfigRequest(Guid Id);

public class RemoveSeniorErpConfigRequestValidator : AbstractValidator<RemoveSeniorErpConfigRequest>
{
    public RemoveSeniorErpConfigRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O Id não pode ser vazio.");
    }
}
