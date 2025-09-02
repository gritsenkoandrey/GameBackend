namespace GameBackend.Scripts.Controllers;

public sealed class AuthResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Error { get; set; }

    public AuthResult(bool success, T data, string error)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    public AuthResult(bool success, T data)
    {
        Success = success;
        Data = data;
        Error = string.Empty;
    }
    
    public AuthResult(bool success, string error)
    {
        Success = success;
        Data = default;
        Error = error;
    }
}