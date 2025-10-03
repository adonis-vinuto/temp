namespace GemelliApi.Domain.ValueObjects;

public sealed class Estado
{
    public static readonly Estado AC = new("AC", "Acre");
    public static readonly Estado AL = new("AL", "Alagoas");
    public static readonly Estado AP = new("AP", "Amapá");
    public static readonly Estado AM = new("AM", "Amazonas");
    public static readonly Estado BA = new("BA", "Bahia");
    public static readonly Estado CE = new("CE", "Ceará");
    public static readonly Estado DF = new("DF", "Distrito Federal");
    public static readonly Estado ES = new("ES", "Espírito Santo");
    public static readonly Estado GO = new("GO", "Goiás");
    public static readonly Estado MA = new("MA", "Maranhão");
    public static readonly Estado MT = new("MT", "Mato Grosso");
    public static readonly Estado MS = new("MS", "Mato Grosso do Sul");
    public static readonly Estado MG = new("MG", "Minas Gerais");
    public static readonly Estado PA = new("PA", "Pará");
    public static readonly Estado PB = new("PB", "Paraíba");
    public static readonly Estado PR = new("PR", "Paraná");
    public static readonly Estado PE = new("PE", "Pernambuco");
    public static readonly Estado PI = new("PI", "Piauí");
    public static readonly Estado RJ = new("RJ", "Rio de Janeiro");
    public static readonly Estado RN = new("RN", "Rio Grande do Norte");
    public static readonly Estado RS = new("RS", "Rio Grande do Sul");
    public static readonly Estado RO = new("RO", "Rondônia");
    public static readonly Estado RR = new("RR", "Roraima");
    public static readonly Estado SC = new("SC", "Santa Catarina");
    public static readonly Estado SP = new("SP", "São Paulo");
    public static readonly Estado SE = new("SE", "Sergipe");
    public static readonly Estado TO = new("TO", "Tocantins");

    public string Sigla { get; }
    public string Nome { get; }

    private Estado(string sigla, string nome)
    {
        Sigla = sigla;
        Nome = nome;
    }

    private Estado() { }

    public static IReadOnlyCollection<Estado> Todos => new List<Estado>
    {
        AC, AL, AP, AM, BA, CE, DF, ES, GO, MA,
        MT, MS, MG, PA, PB, PR, PE, PI, RJ, RN,
        RS, RO, RR, SC, SP, SE, TO
    };

    public static Estado? ObterPorSigla(string sigla)
    {
        return Todos.FirstOrDefault(e => e.Sigla.Equals(sigla, StringComparison.OrdinalIgnoreCase));
    }

    public static Estado Criar(string sigla)
    {
        Estado estado = ObterPorSigla(sigla);
        if (estado is null)
        {
            throw new ArgumentException($"Estado inválido: '{sigla}'");
        }

        return estado;
    }

    public override string ToString() => Sigla;

    public override bool Equals(object? obj)
    {
        return obj is Estado other && Sigla == other.Sigla;
    }

    public override int GetHashCode() => Sigla.GetHashCode();
}