namespace GemelliApi.Domain.Enums;

public enum UserRoles
{
    Administrador,
    Gerente,
    Desenvolvedor,
    Analista,
    Estagiario
}

public static class RoleIds
{
    public const string Administrador = "1d63a6b8-4c0c-4a09-a4c3-65e6b84c0a21";
    public const string Gerente = "9b91cf0e-7ad3-4076-b7de-2b70a1b14e45";
    public const string Desenvolvedor = "4e2eaa02-67f4-4f7e-8ec1-376b7a1243f1";
    public const string Analista = "fcb4eae6-17d7-4d93-b9e6-4209d6d46fa1";
    public const string Estagiario = "7ad5a5a9-1b4c-4bcb-8a39-9bde0d7d8f52";
}