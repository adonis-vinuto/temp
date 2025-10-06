using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Handlers.SeniorHcmConfig.SearchById;
public record SearchByIdSeniorHcmConfigRequest(Guid Id);
public class SearchByIdSeniorHcmConfigRequestValidator : AbstractValidator<SearchByIdSeniorHcmConfigRequest>
{
    public SearchByIdSeniorHcmConfigRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O Id não pode ser vazio.");
    }
}

