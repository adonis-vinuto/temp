using FluentValidation;

namespace Application.Handlers.Ai.Agent.SearchByIdAndOrganization;

public record SearchByIdAndOrganizationRequest(Guid Id);

public class SearchByIdAndOrganizationRequestValidator : AbstractValidator<SearchByIdAndOrganizationRequest>
{
    public SearchByIdAndOrganizationRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID não pode ser vazio.");
    }
}