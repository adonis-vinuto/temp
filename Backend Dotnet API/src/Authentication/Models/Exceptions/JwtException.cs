namespace Authentication.Models.Exceptions;

public class JwtException : Exception
{
    public JwtException()
    {
    }

    public JwtException(string mensagem) : base(mensagem)
    {
    }

    public JwtException(string mensagem, Exception exception) : base(mensagem, exception)
    {
    }
}