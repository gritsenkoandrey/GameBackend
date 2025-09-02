namespace GameBackend.Scripts.Settings;

public sealed class AuthSettings
{
    public TimeSpan Expires { get; set; }
    public string SecretKey { get; set; }
}