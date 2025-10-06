using System.Text.RegularExpressions;

namespace GemelliApi.Domain.ValueObjects;

public sealed class Email
{
    public string Endereco { get; private set; }

    public Email(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
        {
            throw new ArgumentException("Email não pode ser vazio.");
        }

        if (!Regex.IsMatch(endereco, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Email inválido.");
        }

        Endereco = endereco;
    }

    public override bool Equals(object? obj)
    {
        return obj is Email email &&
               string.Equals(Endereco, email.Endereco, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Endereco);
    }

    public override string ToString() => Endereco;
}