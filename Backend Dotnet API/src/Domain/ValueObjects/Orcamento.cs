using GemelliApi.Domain.Exceptions;

namespace GemelliApi.Domain.ValueObjects;

public sealed class Orcamento
{
    public decimal Valor { get; }
    public string Moeda { get; } = "BRL";

    public Orcamento(decimal valor)
    {
        if (valor <= 0)
        {
            throw new DomainException("Orçamento deve ser maior que zero");
        }

        Valor = valor;
    }

    public static implicit operator decimal(Orcamento orcamento) => orcamento.Valor;
}