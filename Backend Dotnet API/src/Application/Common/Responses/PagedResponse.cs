using Domain.Entities;

namespace Application.Common.Responses;

public record PagedResponse<T>(
    int TotalPaginas,
    int TotalItens,
    int Indice,
    int TamanhoPagina,
    IReadOnlyCollection<T> Itens)
{

    public static PagedResponse<T> Create(
        IEnumerable<T> itens,
        int totalItens,
        int indice,
        int tamanhoPagina)
    {
        int totalPaginas = tamanhoPagina <= 0
            ? 0
            : (int)Math.Ceiling(totalItens / (double)tamanhoPagina);

        return new PagedResponse<T>(
            totalPaginas,
            totalItens,
            indice,
            tamanhoPagina,
            itens.ToList().AsReadOnly());
    }
}
