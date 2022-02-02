using Microsoft.Graph;

namespace GSoft.Password.Rotate;

public class AppQueries
{
    public static async Task<IReadOnlyCollection<Guid>> RetrieveList(GraphServiceClient graphServiceClient)
    {
        var application = graphServiceClient.Applications;
        var request = application.Request();
        var allApps = await GetAllApplications(request).Select(x => Guid.Parse(x.AppId)).ToListAsync();
        return allApps;
    }

    public async static IAsyncEnumerable<Application> GetAllApplications(IGraphServiceApplicationsCollectionRequest request)
    {
        var current = request;
        while (current != null)
        {
            var page = await current.GetAsync();
            foreach (var application in page.CurrentPage)
            {
                yield return application;
            }
            current = page.NextPageRequest;
        }

    }
}    
    
    // public static string RetrieveList()
    // {
    //     const string strCmdText = "-c az ad app list --show-mine -o json --query '[].appId' | jq -c '.[]'  | xargs -L 1";
    //     return System.Diagnostics.Process.Start("/bin/bash",strCmdText).ToString();
    // }
