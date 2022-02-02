namespace GSoft.Password.Rotate;
public class SecretRotationEntryOptions
{
    public SecretRotationEntrySourceOptions Source { get; set; }

    public SecretRotationEntryDestinationOptions Destination { get; set; }
}

public class SecretRotationEntrySourceOptions
{
    public Guid AppId { get; set; }
}

public class SecretRotationEntryDestinationOptions
{
    public Uri KeyVaultUri { get; set; }

    public string SecretName { get; set; }
}