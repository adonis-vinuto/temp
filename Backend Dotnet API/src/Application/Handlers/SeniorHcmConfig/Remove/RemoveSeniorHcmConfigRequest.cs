using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Handlers.SeniorHcmConfig.Remove;

public record RemoveSeniorHcmConfigRequest(Guid Id);

public class RemoveSeniorHcmConfigRequestValidator : AbstractValidator<RemoveSeniorHcmConfigRequest>
{
    public RemoveSeniorHcmConfigRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O Id não pode ser vazio.");
    }
}

