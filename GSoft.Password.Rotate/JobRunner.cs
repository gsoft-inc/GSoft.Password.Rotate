using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            var rotationOptions = this._configuration.GetSection("SecretRotations").Get<SecretRotationEntryOptions[]>();

            var graphClient = new GraphServiceClient(new AzureCliCredential());
            var azureAdHttpClient = new AzureAdAppClient(graphClient);

            foreach (var (app, sink) in rotationOptions)
            {
                if (!string.IsNullOrEmpty(sink.SecretName))
                {
                    var secret = await azureAdHttpClient.CreateSecret(app.ObjectId);
                    var keyVaultClient = new SecretClient(sink.KeyVaultUri, new AzureCliCredential());

                    await keyVaultClient.SetSecretAsync(sink.SecretName, secret.SecretText, stoppingToken);

                    this._logger.LogInformation("Password set secret result {SetSecretResult}",JsonSerializer.Serialize(secret));
                }

                await azureAdHttpClient.UpdateApplication(app.ObjectId);
            }
        }
        finally
        {
            this._hostApplicationLifetime.StopApplication();
        }
    }
}