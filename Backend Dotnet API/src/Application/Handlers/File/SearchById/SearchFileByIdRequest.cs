using FluentValidation;

namespace Application.Handlers.File.SearchById;

public record SearchFileByIdRequest(Guid Id);

public class SearchFileByIdRequestValidator : AbstractValidator<SearchFileByIdRequest>
{
    public SearchFileByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
                .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
                .WithMessage("ID não pode ser vazio.")
            .Must(x => Guid.TryParse(x.ToString(), out _))
                .WithMessage("ID inválido.");
    }
}