using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Domain.Helpers;

public static class DateTimeProvider
{
    public static DateTime DataHoraAtual() =>DateTime.UtcNow;

    public static DateTime ConverterDataHora(DateTime dataHora) =>
        dataHora.ToUniversalTime();
}
