using FluentValidation;

namespace Application.Handlers.DataConfig.SearchById;

public record SearchDataConfigByIdRequest(
    Guid Id
);

public class SearchDataConfigByIdRequestValidator : AbstractValidator<SearchDataConfigByIdRequest>
{
    public SearchDataConfigByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("O ID não pode ser nulo.")
            .NotEmpty()
            .WithMessage("O ID não pode ser vazio.");
    }
}