namespace Authentication.Models;

public record UsuarioToken(string Id, string Usuario, string Email, IEnumerable<UsuarioClaim> Claims);

public record UsuarioClaim(string Valor, string Tipo);