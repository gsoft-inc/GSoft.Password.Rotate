using Microsoft.Graph;

namespace GSoft.Password.Rotate;

public class AzureAdAppClient
{
    private readonly GraphServiceClient _graphServiceClient;

    public AzureAdAppClient(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    public Task<PasswordCredential> CreateSecret(Guid objectId)
    {
        var application = _graphServiceClient.Applications[objectId.ToString()];
        var request = application.AddPassword(new PasswordCredential()
        {
            StartDateTime = DateTimeOffset.UtcNow,
            EndDateTime = DateTimeOffset.UtcNow.AddMonths(1)
             
        }).Request();

        return request.PostAsync();
    }
}