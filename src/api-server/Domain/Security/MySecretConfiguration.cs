namespace Domain.Security;

public static class MySecretConfiguration
{
    public static SecretsConfiguration Secrets { get; set; } = new();
    public class SecretsConfiguration
    {
        public string JwtPrivateKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}