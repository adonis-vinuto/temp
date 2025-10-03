namespace API.Models.Response;

public class BaseResponse
{
    public bool Status { get; private set; }
    public object? Data { get; private set; }

    public static BaseResponse Sucesso(object? resultado = null)
    {
        return new BaseResponse
        {
            Data = resultado,
            Status = true
        };
    }
}