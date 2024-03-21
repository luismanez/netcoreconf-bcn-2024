using Azure.Identity;
using CozyKitchen.Extensions;
using CozyKitchen.Plugins.Native;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Beta;
using Microsoft.SemanticKernel;

namespace CozyKitchen.HostedServices;
public class NestedFunctionHostedService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly Kernel _kernel;
    private KernelPlugin _prompts;

    public NestedFunctionHostedService(
        ILogger<NestedFunctionHostedService> logger,
        IConfiguration configuration,
        Kernel kernel)
    {
        _logger = logger;
        _configuration = configuration;
        _kernel = kernel;
        _prompts = _kernel.CreatePluginFromPromptDirectory(
            $"{PathExtensions.GetPluginsRootFolder()}/ResumeAssistantPlugin");
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var graphClient = GetGraphServiceClient();

        // need to import the Native pluggin, as will be used as nested function of the main Semantic function
        var graphSkillsPlugin = new GraphUserProfileSkillsPlugin(graphClient);
        _ = _kernel.Plugins.AddFromObject(graphSkillsPlugin, "GraphSkillsPlugin");

        var mySkillsInfo = await _kernel.InvokeAsync<string>(
            _prompts["MySkillsDefinition"]
        );

        _logger.LogInformation($"-----MY SKILLS-----\n{mySkillsInfo}");
    }

    private GraphServiceClient GetGraphServiceClient()
    {
        var scopes = new[] { "User.Read" };
        var clientId = _configuration.GetValue<string>("AzureAd:ClientId");
        var tenantId = _configuration.GetValue<string>("AzureAd:TenantId");

        var options = new InteractiveBrowserCredentialOptions
        {
            TenantId = tenantId,
            ClientId = clientId,
            RedirectUri = new Uri("http://localhost"),
        };

        var interactiveCredential = new InteractiveBrowserCredential(options);
        var graphClient = new GraphServiceClient(interactiveCredential, scopes);

        return graphClient;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("HostedService Stopped");
        return Task.CompletedTask;
    }
}
