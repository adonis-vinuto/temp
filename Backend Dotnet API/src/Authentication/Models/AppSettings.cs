namespace Authentication.Models;

public class AppSettings
{
    public JwtSettings Jwt { get; set; }
}

public class JwtSettings
{
    public string Key { get; set; } = "Dq5KfwlKDxLJH9BdTUpob+btR2Qo1EReTVFuw27o3hA="; // Essa chave fica aqui apenas para que os testes de integração funcionem. Em desenvolvimento ou produção, ela é sobrescrita pela chave que estiver no secret ou key vault
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationInHours { get; set; }
}