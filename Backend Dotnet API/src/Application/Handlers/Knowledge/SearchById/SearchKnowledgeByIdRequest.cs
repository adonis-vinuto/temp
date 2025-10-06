using FluentValidation;

namespace Application.Handlers.Knowledge.SearchById;

public record SearchKnowledgeByIdRequest(Guid Id);

public class SearchKnowledgeByIdRequestValidator : AbstractValidator<SearchKnowledgeByIdRequest>
{
    public SearchKnowledgeByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID não pode ser vazio.");
    }
}