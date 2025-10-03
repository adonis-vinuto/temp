using System.Text.RegularExpressions;

namespace Domain.Extensions;

public static class StringExtensions
{
    public static bool EmailValido(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        email = email.Trim();

        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        var regex = new Regex(pattern, RegexOptions.Compiled);
        return regex.IsMatch(email);
    }

    public static string SeparaStringPascalCase(this string input)
    {
        string output = Regex.Replace(input, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
        return output.Trim();
    }

    public static bool ApenasNumeros(this string input)
    {
        string inputSemPontos = input.Replace(".", "");
        const string pattern = @"^[0-9]*$";
        var regex = new Regex(pattern, RegexOptions.Compiled);
        return regex.IsMatch(inputSemPontos);
    }
}
