using Domain.Extensions;

namespace Application.Extensions;

public static class EnumExtensions
{
    public static string ConverterEnumEmString(this Enum tipoDistribuicaoEnum)
    {
        return tipoDistribuicaoEnum.ToString().SeparaStringPascalCase();
    }
}