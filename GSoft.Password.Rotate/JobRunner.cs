using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace GSoft.Password.Rotate;

public class JobRunner : BackgroundService
{
    private readonly ILogger<JobRunner> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public JobRunner(ILogger<JobRunner> logger, IConfiguration configuration, IHostApplicationLifetime hostApplicationLifetime)
    {
        this._logger = logger;
        this._configuration = configuration;
        this._hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var options = this._configuration.GetSection(SecretSourceOptions.SectionName).Get<SecretSourceOptions>();

            var graphClient = new GraphServiceClient(new AzureCliCredential());
            var azureAdHttpClient = new AzureAdHttpClient(graphClient);
            var appId = Guid.Parse("SubscriptionId");
            var secretName = RandomSecretName.RandomString(10);
            var result = await azureAdHttpClient.SetSecret(appId, secretName);
            var keyVaultClient = new SecretClient(new Uri("https://keyvaultUri.vault.azure.net"), new AzureCliCredential());

            var idList = await AppQueries.RetrieveList(graphClient);

            await keyVaultClient.SetSecretAsync(secretName, keyVaultClient.ToString(), stoppingToken);

            this._logger.LogInformation("App IDs: '{AppIds}'",string.Join(", ", idList));
            this._logger.LogInformation("Password set secret result {SetSecretResult}",JsonSerializer.Serialize(result));

        }
        finally
        {
            this._hostApplicationLifetime.StopApplication();
        }
    }
}