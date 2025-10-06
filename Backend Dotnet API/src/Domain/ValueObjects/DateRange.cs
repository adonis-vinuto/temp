using GemelliApi.Domain.Exceptions;

namespace GemelliApi.Domain.ValueObjects;

public sealed class DateRange
{
    public DateOnly Inicio { get; }
    public DateOnly Termino { get; }

    public DateRange(DateOnly inicio, DateOnly termino)
    {
        if (termino < inicio)
        {
            throw new DomainException("Data de término não pode ser anterior ao início");
        }

        Inicio = inicio;
        Termino = termino;
    }

    public int DuracaoEmDias => Termino.DayNumber - Inicio.DayNumber + 1;

    public bool Sobrepoe(DateRange outroPeriodo) =>
        Inicio <= outroPeriodo.Termino && Termino >= outroPeriodo.Inicio;
}