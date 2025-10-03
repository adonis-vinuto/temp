using FluentValidation;

namespace Application.Handlers.Knowledge.CreateImportSenior;

public record CreateImportSeniorRequest(
    Guid IdKnowledge,
    Guid IdSeniorHcmConfig
);


public class CreateKnowledgeRequestValidator : AbstractValidator<CreateImportSeniorRequest>
{
    public CreateKnowledgeRequestValidator()
    {
        RuleFor(x => x.IdKnowledge)
            .NotEmpty().WithMessage("IdKnowledge é necessário.");

        RuleFor(x => x.IdSeniorHcmConfig)
            .NotEmpty().WithMessage("IdSeniorHcmConfig é necessário.");
    }
}
