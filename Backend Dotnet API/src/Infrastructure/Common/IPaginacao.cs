namespace Infrastructure.Common;

public interface IPaginacao<T>
{
    int TotalDePaginas { get; }
    int Indice { get; }
    int TotalDeItens { get; }

    Task<Paginacao<T>> CriarAsync(IQueryable<T> consulta, int indice, int tamanhoPagina);
}