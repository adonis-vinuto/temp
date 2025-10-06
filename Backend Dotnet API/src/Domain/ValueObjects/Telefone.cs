using System.Text.RegularExpressions;

namespace GemelliApi.Domain.ValueObjects;

public sealed class Telefone
{
    public string Numero { get; private set; }

    public Telefone(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
        {
            throw new ArgumentException("Telefone não pode ser vazio.");
        }

        if (!Regex.IsMatch(numero, @"^\d{10,15}$"))
        {
            throw new ArgumentException("Telefone inválido. Deve conter entre 10 e 15 dígitos.");
        }

        Numero = numero;
    }

    public override string ToString() => Numero;

    public override bool Equals(object? obj)
    {
        return obj is Telefone telefone && Numero == telefone.Numero;
    }

    public override int GetHashCode() => Numero.GetHashCode();
}