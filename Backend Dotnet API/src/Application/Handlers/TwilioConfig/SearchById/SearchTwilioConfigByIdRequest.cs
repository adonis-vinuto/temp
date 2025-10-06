using FluentValidation;

namespace Application.Handlers.TwilioConfig.SearchById;

public record SearchTwilioConfigByIdRequest(Guid Id);

public class SearchTwilioConfigByIdRequestValidator : AbstractValidator<SearchTwilioConfigByIdRequest>
{
    public SearchTwilioConfigByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório.");
    }
}
