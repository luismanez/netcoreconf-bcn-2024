using System.ComponentModel;
using Microsoft.Graph.Beta;
using Microsoft.SemanticKernel;

namespace CozyKitchen.Plugins.Native;
public class GraphUserProfileSkillsPlugin
{
    private readonly GraphServiceClient _client;

    public GraphUserProfileSkillsPlugin(GraphServiceClient client)
    {
        _client = client;
    }

    [KernelFunction, Description("Get current user's skills in their profile in MS Graph")]
    public async Task<string> GetMySkills()
    {
        var data = await _client.Me.Profile.Skills.GetAsync(q =>
        {
            q.QueryParameters.Top = 3;
            q.QueryParameters.Orderby = new string[] { "createdDateTime" };
            q.QueryParameters.Select = new string[] { "id", "displayName" };
        });

        var skills = data!.Value!.Select(s => s.DisplayName)!;

        return string.Join(",", skills);
    }
}
