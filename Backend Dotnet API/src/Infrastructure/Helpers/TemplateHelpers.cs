namespace GemelliApi.Infrastructure.Helpers.TemplateHelper;

public static class TemplateHelper
{
    // Substitui as variáveis dos arquivos html de emails
    public static string ReplacePlaceholders(string template, IDictionary<string, string> values)
    {
        if (string.IsNullOrWhiteSpace(template) || values == null)
        {
            return template;
        }

        return values.Aggregate(template, (current, pair) =>
                current.Replace($"{{{pair.Key}}}", pair.Value ?? string.Empty));
    }
}