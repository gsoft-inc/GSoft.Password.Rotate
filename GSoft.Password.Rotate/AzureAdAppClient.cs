using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

    public async Task<Application> UpdateApplication(Guid objectId)
    {
        var application = await _graphServiceClient.Applications[objectId.ToString()].Request().GetAsync();
        var keyCredentials = new List<KeyCredential>(application.KeyCredentials);

        var key = CertificateUtil.MakeCert();

        keyCredentials.Add(new KeyCredential()
        {
            Key = key,
            DisplayName = "yolo",
            Usage = "Verify",
            Type = "AsymmetricX509Cert"
        });

        application.KeyCredentials = keyCredentials;

        return await _graphServiceClient.Applications[objectId.ToString()].Request().UpdateAsync(application);
    }

    private static class CertificateUtil
    {
        public static byte[] MakeCert()
        {
            var ecdsa = ECDsa.Create(); // generate asymmetric key pair
            var certificateRequest = new CertificateRequest("cn=foobar", ecdsa, HashAlgorithmName.SHA256);
            var cert = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

            return cert.GetRawCertData();
        }
    }
}