namespace Application.DTOs.Knowledge.ImportSenior;

public class Senior
{
    public readonly String Uri;

    public readonly String LoginSenior;

    public readonly String PasswordSenior;
    
    public readonly String Service;
    
    public readonly String Parameters;

    public readonly int Encryption;

    public readonly String DataReferencia;

    public Senior(String? uri, String? loginSenior, String? passwordSenior, String? service, String? parameters, String? dataReferencia)
    {
        Uri = uri ?? String.Empty;
        LoginSenior = loginSenior ?? String.Empty;
        PasswordSenior = passwordSenior ?? String.Empty;
        Service = service ?? String.Empty;
        Parameters = parameters ?? String.Empty;
        Encryption = 0;
        DataReferencia = dataReferencia ?? String.Empty;
    }
}
