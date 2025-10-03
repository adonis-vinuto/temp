using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Handlers.Knowledge.ImportExcel;

public record ImportExcelKnowledgeRequest(
    IFormFile ExcelFile
)
{
    [JsonIgnore]
    public Guid IdKnowledge { get; set; }
}


public class ImportExcelKnowledgeRequestValidator : AbstractValidator<ImportExcelKnowledgeRequest>
{
    public ImportExcelKnowledgeRequestValidator()
    {
        RuleFor(x => x.IdKnowledge)
            .NotEmpty().WithMessage("Id do Knowledge é necessário.");

        RuleFor(x => x.ExcelFile)
            .NotNull().WithMessage("Arquivo Excel é necessário.")
            .Must(file => file != null && file.Length > 0).WithMessage("Arquivo Excel não pode estar vazio.");
    }
}
