using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;

namespace Application.Handlers.File.Create;

public record CreateFileRequest(
    IFormFile Arquivo,
    string Organization,
    string IdAgent,
    Guid? IdFile
);

public class CreateFileRequestValidator : AbstractValidator<CreateFileRequest>
{
    private static readonly Dictionary<string, long> LimitesTamanho = new()
    {
        [".txt"] = 1 * 1024 * 1024,
        [".doc"] = 5 * 1024 * 1024,
        [".docx"] = 5 * 1024 * 1024,
        [".xls"] = 5 * 1024 * 1024,
        [".xlsx"] = 5 * 1024 * 1024,
        [".pdf"] = 15 * 1024 * 1024
    };

    public CreateFileRequestValidator()
    {
        RuleFor(x => x.Arquivo)
            .NotEmpty()
            .WithMessage("Um arquivo deve ser anexado.")
            .Must(ArquivoPermitido)
            .WithMessage("Tipos de arquivo não permitidos.")
            .Must(TamanhoValido)
            .WithMessage(x => $"O arquivo excede o tamanho máximo de {ObterLimite(x.Arquivo)} MB.");
    }

    private static bool ArquivoPermitido(IFormFile file)
    {
        string extensao = ObterExtensao(file.FileName);
        return LimitesTamanho.ContainsKey(extensao);
    }

    private static bool TamanhoValido(IFormFile file)
    {
        string extensao = ObterExtensao(file.FileName);
        return LimitesTamanho.TryGetValue(extensao, out long limite) && file.Length <= limite;
    }

    private static string ObterLimite(IFormFile file)
    {
        string extensao = ObterExtensao(file.FileName);
        return LimitesTamanho.TryGetValue(extensao, out long limite)
            ? (limite / (1024 * 1024)).ToString(System.Globalization.CultureInfo.CurrentCulture)
            : "0";
    }

    private static string ObterExtensao(string fileName)
    {
        return Path.GetExtension(fileName)?.ToLower(System.Globalization.CultureInfo.CurrentCulture) ?? string.Empty;
    }
}
