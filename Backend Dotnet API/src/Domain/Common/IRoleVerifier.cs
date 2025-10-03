namespace Domain.Common;

public interface IRoleVerifier
{
    /// <summary>
    /// Verifica se o <c>Usuario</c> possui o cargo de Coordenador de acordo com o <paramref name="idIdentityUsuario"/>.
    /// </summary>
    /// <param name="idIdentityUsuario">O Id do usuário (IdentityID).</param>
    /// <returns>'true' se o usuário possuir o cargo de coordenador e 'false' se não possuir ou não for encontrado.</returns>
    bool IsUserInCoordenadorRole(string idIdentityUsuario);
}