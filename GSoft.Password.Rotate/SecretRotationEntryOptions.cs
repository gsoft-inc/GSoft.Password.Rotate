namespace GSoft.Password.Rotate;
public class SecretRotationEntryOptions
{
    public AzureAdAppOptions AzureAdApp { get; set; }

    public KeyVaultSecretOptions Sink { get; set; }

    public void Deconstruct(out AzureAdAppOptions app, out KeyVaultSecretOptions sink)
    {
        app = AzureAdApp;
        sink = Sink;
    }
}

public class AzureAdAppOptions
{
    public Guid ObjectId { get; set; }
}

public class KeyVaultSecretOptions
{
    public Uri KeyVaultUri { get; set; }

    public string SecretName { get; set; }
}