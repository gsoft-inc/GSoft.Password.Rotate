namespace GSoft.Password.Rotate;

public class SecretSourceOptions
{
    public const string SectionName = "SecretSource";

    public string[] AzureAppIds { get; set; }
}