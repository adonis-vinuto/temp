namespace GemelliApi.Domain.Exceptions;

public class DomainValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    public DomainValidationException(Dictionary<string, string[]> errors)
        : base("Ocorreram erros de validação")
    {
        Errors = errors;
    }

    public DomainValidationException(string property, string error)
        : base("Erro de validação")
    {
        Errors = new Dictionary<string, string[]>
        {
            { property, new[] { error } }
        };
    }
}