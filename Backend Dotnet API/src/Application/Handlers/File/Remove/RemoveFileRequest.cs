using FluentValidation;

namespace Application.Handlers.File.Remove;

public sealed record RemoveFileRequest(Guid Id);

public class RemoveFileRequestValidator : AbstractValidator<RemoveFileRequest>
{
    public RemoveFileRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
                .WithMessage("ID não deve ser nulo.")
            .NotEmpty()
                .WithMessage("ID não pode ser vazio.")
            .Must(x => Guid.TryParse(x.ToString(), out _))
                .WithMessage("ID inválido.");
    }
}