using FluentValidation;

namespace Application.Handlers.File.Search;

public record SearchFilesRequest(
    Guid? IdAgent,
    int Pagina = 1,
    int TamanhoPagina = 10
);


public class SearchFilesRequestValidator : AbstractValidator<SearchFilesRequest>
{
    public SearchFilesRequestValidator()
    {
        When(x => x.IdAgent != null, () =>
            RuleFor(x => x.IdAgent)
                .Must(x => Guid.TryParse(x.ToString(), out _))
                .WithMessage("Id de agente inválido.")
        );

        RuleFor(x => x.Pagina)
            .GreaterThan(0).WithMessage("Página deve ser maior que zero.");

        RuleFor(x => x.TamanhoPagina)
            .GreaterThan(0).WithMessage("Tamanho da página deve ser maior que zero.");
    }
}
