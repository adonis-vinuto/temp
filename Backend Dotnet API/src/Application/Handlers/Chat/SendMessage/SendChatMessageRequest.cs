using FluentValidation;

namespace Application.Handlers.Chat.SendMessage;

public record SendChatMessageRequest(Guid IdAgent, Guid IdSession, string Message);

public class SendChatMessageRequestValidator : AbstractValidator<SendChatMessageRequest>
{
    public SendChatMessageRequestValidator()
    {
        RuleFor(x => x.IdAgent)
            .NotEmpty()
            .WithMessage("ID do Agente é obrigatório.");

        RuleFor(x => x.IdSession)
            .NotEmpty()
            .WithMessage("ID da Sessão é obrigatório.");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Mensagem é obrigatória.")
            .MaximumLength(5000)
            .WithMessage("Mensagem não pode exceder 5000 caracteres.");
    }
}