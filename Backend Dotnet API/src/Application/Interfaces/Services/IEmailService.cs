namespace Application.Interfaces.Services;

public interface IEmailService
{
    Task EnviarEmail(string remetente, string destinatario, string titulo, string corpoHtml,
        CancellationToken cancellationToken);

    string LoadTemplate(string templateFileName);

    string SubstituirVariaveisEmailEsqueciSenha(
            string? arquivo,
            string? urlBase,
            string? token,
            string? nomeUsuario,
            string? usuarioLogin);

    string SubstituirVariaveisEmailEsqueciSenha(
            string? arquivo,
            Uri? urlBase,
            string? token,
            string? nomeUsuario,
            string? usuarioLogin);
}
