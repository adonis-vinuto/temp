using System.ComponentModel;

namespace Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        System.Reflection.FieldInfo? field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute)field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
        return attribute?.Description ?? value.ToString();
    }

    public static TEnum FromDescription<TEnum>(string description) where TEnum : Enum
    {
        foreach (System.Reflection.FieldInfo field in typeof(TEnum).GetFields())
        {
            var attribute = (DescriptionAttribute)field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            if (attribute?.Description.Equals(description, StringComparison.OrdinalIgnoreCase) == true)
            {
                return (TEnum)field.GetValue(null);
            }
        }
        throw new ArgumentException($"Descrição inválida para o enum {typeof(TEnum).Name}: {description}");
    }
}
