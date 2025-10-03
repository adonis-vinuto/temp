using FluentValidation;

namespace Application.Handlers.Chat.FirstMessage;

public record FirstMessageRequest(Guid IdAgent, string Message);

public class FirstMessageRequestValidator : AbstractValidator<FirstMessageRequest>
{
    public FirstMessageRequestValidator()
    {
        RuleFor(x => x.IdAgent)
            .NotEmpty()
            .WithMessage("ID do Agente é obrigatório.");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Mensagem é obrigatória.")
            .MaximumLength(5000)
            .WithMessage("Mensagem não pode exceder 5000 caracteres.");
    }
}