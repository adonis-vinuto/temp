using FluentValidation;

namespace Application.Handlers.ChatHistory.SearchByIdAgent;

public record SearchChatHistoryByIdAgentRequest(Guid IdSession);

public class SearchChatHistoryByIdAgentRequestValidator : AbstractValidator<SearchChatHistoryByIdAgentRequest>
{
    public SearchChatHistoryByIdAgentRequestValidator()
    {
        RuleFor(x => x.IdSession)
            .NotNull()
            .WithMessage("ID da Sessão não deve ser nulo.")
            .NotEmpty()
            .WithMessage("ID da Sessão não pode ser vazio.");
    }
}