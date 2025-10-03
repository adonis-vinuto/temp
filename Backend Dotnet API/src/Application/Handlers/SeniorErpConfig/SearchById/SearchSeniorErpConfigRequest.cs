using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.SeniorErpConfig.SearchById;

public record SearchByIdSeniorErpConfigRequest(Guid Id);

public class SearchByIdSeniorErpConfigRequestValidator : AbstractValidator<SearchByIdSeniorErpConfigRequest>
{
    public SearchByIdSeniorErpConfigRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O Id não pode ser vazio.");
    }
}

