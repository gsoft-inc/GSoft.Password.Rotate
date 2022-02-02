// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using GSoft.Password.Rotate;
using Microsoft.Graph;

var graphClient = new GraphServiceClient(new AzureCliCredential());
var azureAdHttpClient = new AzureAdHttpClient(graphClient);
var appId = Guid.Parse("SubscriptionId");
var secretName = RandomSecretName.RandomString(10);
var result = await azureAdHttpClient.SetSecret(appId, secretName);
var keyvaultClient = new SecretClient(new Uri("https://keyvaultUri.vault.azure.net"), new AzureCliCredential());


var idList = await AppQueries.RetrieveList(graphClient);



keyvaultClient.SetSecret(secretName, keyvaultClient.ToString());

Console.WriteLine(string.Join(", ", idList));
Console.WriteLine(JsonSerializer.Serialize(result));