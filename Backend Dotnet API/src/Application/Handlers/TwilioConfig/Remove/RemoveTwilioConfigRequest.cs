using FluentValidation;

namespace Application.Handlers.TwilioConfig.Remove;

public record RemoveTwilioConfigRequest(Guid Id);

public class RemoveTwilioConfigRequestValidator : AbstractValidator<RemoveTwilioConfigRequest>
{
    public RemoveTwilioConfigRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório.");
    }
}
