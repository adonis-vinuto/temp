using FluentValidation;

namespace Application.Handlers.Agent.SearchById;

public record SearchAgentByIdRequest(Guid Id);

public class SearchAgentByIdRequestValidator : AbstractValidator<SearchAgentByIdRequest>
{
    public SearchAgentByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID não pode ser vazio.");
    }
}