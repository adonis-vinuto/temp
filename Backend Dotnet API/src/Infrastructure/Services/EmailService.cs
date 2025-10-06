using SendGrid;
using SendGrid.Helpers.Mail;
using Application.AppConfig;
using Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GemelliApi.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly IWebHostEnvironment _env;
    private readonly TemplateEmailSettings _templateSettings;
    private readonly ILogger<EmailService> _logger;
    public EmailService(IOptions<SmtpSettings> smtpOpts, IWebHostEnvironment env, IOptions<TemplateEmailSettings> templateOpts, ILogger<EmailService> logger)
    {
        _client = new SendGridClient(smtpOpts.Value.Senha);
        _env = env;
        _templateSettings = templateOpts.Value;
        _logger = logger;
    }

    public async Task EnviarEmail(string remetente, string destinatario, string titulo, string corpoHtml,
        CancellationToken cancellationToken)
    {
        var de = new EmailAddress(remetente, "GemelliApi Web");
        var para = new EmailAddress(destinatario);
        SendGridMessage msg = MailHelper.CreateSingleEmail(de, para, titulo, plainTextContent: null, htmlContent: corpoHtml);
        Response resposta = await _client.SendEmailAsync(msg, cancellationToken);

        if ((int)resposta.StatusCode >= 400)
        {
            string body = await resposta.Body.ReadAsStringAsync(cancellationToken);

            _logger.LogError(
                "Erro ao enviar email. Para: {Destinatario}, Assunto: {Assunto}, StatusCode: {StatusCode}, Body: {Body}",
                destinatario, titulo, (int)resposta.StatusCode, body);

            throw new Exception($"Falha no envio de e-mail: {resposta.StatusCode} – {body}");
        }
    }

    public string LoadTemplate(string templateFileName)
    {
        string folder = _templateSettings.BasePath;
        string templatePath = Path.Combine(_env.ContentRootPath, folder, templateFileName);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template não encontrado em {templatePath}");
        }

        return File.ReadAllText(templatePath);
    }

    public string SubstituirVariaveisEmailEsqueciSenha(
        string? arquivo,
        string? urlBase,
        string? token,
        string? nomeUsuario,
        string? usuarioLogin)
    {
        if (string.IsNullOrWhiteSpace(arquivo))
        {
            return string.Empty;
        }

        string templateEmail = arquivo
            .Replace("{URL_BASE}", urlBase)
            .Replace("{USUARIO_LOGIN}", usuarioLogin)
            .Replace("{NOME_USUARIO}", nomeUsuario)
            .Replace("{TOKEN}", token);

        return templateEmail;
    }

    public string SubstituirVariaveisEmailEsqueciSenha(string? arquivo, Uri? urlBase, string? token, string? nomeUsuario, string? usuarioLogin)
    {
        if (string.IsNullOrWhiteSpace(arquivo))
        {
            return string.Empty;
        }

        string templateEmail = arquivo
            .Replace("{URL_BASE}", urlBase?.OriginalString)
            .Replace("{USUARIO_LOGIN}", usuarioLogin)
            .Replace("{NOME_USUARIO}", nomeUsuario)
            .Replace("{TOKEN}", token);

        return templateEmail;
    }
}
