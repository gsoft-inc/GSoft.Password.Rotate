using Microsoft.Graph;

namespace GSoft.Password.Rotate;

public class AzureAdHttpClient
{
    private readonly GraphServiceClient _graphServiceClient;

    public AzureAdHttpClient(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    public async Task<PasswordCredential> SetSecret(Guid appId, string secretName)
    {
        var application = _graphServiceClient.Applications[appId.ToString()];
        var request = application.AddPassword(new PasswordCredential()
        {
            DisplayName = secretName,
            StartDateTime = DateTimeOffset.UtcNow,
            EndDateTime = DateTimeOffset.UtcNow.AddMonths(1)
             
        }).Request();
        var response = await request.PostAsync();
        return response;
    }
}