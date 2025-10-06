using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public class Paginacao<T> : List<T>, IPaginacao<T>
{
    public int TotalDePaginas { get; }
    public int Indice { get; }
    public int TotalDeItens { get; }

    public Paginacao()
    {
    }

    public Paginacao(List<T> itens, int totalDeItens, int indice, int tamanhoPagina)
    {
        Indice = indice;
        TotalDeItens = totalDeItens;
        TotalDePaginas = (int)Math.Ceiling(totalDeItens / (double)tamanhoPagina);
        AddRange(itens);
    }

    public async Task<Paginacao<T>> CriarAsync(IQueryable<T> consulta, int indice, int tamanhoPagina)
    {
        int quantidadeDeItens = await consulta.CountAsync();
        List<T> itens = await consulta.Skip((indice - 1) * tamanhoPagina).Take(tamanhoPagina).ToListAsync();
        return new Paginacao<T>(itens, quantidadeDeItens, indice, tamanhoPagina);
    }
}
