using System.ComponentModel;
using CozyKitchen.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace CozyKitchen;

public class UserProfile
{
    private readonly ILogger _logger;
    private readonly Kernel _kernel;

    public UserProfile(ILogger<UserProfile> logger, Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel;
    }

    [KernelFunction]
    [Description("Get the user's city name (i.e: New York)")]
    public string GetUserCity(
        [Description("The user's unique identifier")]
        string userId)
    {
        _logger.LogInformation("Getting user city for {userId}", userId);
        return "Valencia";
    }

    [KernelFunction]
    [Description("Get the user's city name (i.e: New York)")]
    public string GetUserPhoneNumer(
        [Description("The user's unique identifier")]
        string userId)
    {
        _logger.LogInformation("Getting user phone number for {userId}", userId);
        return "+34 123 456 7890";
    }

    public async Task<IAgent> GetAgent()
    {
        var openAiOptions = _kernel.Services.GetRequiredService<IOptions<OpenAiOptions>>()!.Value;

        var plugin = KernelPluginFactory.CreateFromObject(this);

        return await new AgentBuilder()
            .WithAzureOpenAIChatCompletion(
                        endpoint: openAiOptions.ApiEndpoint,
                        model: openAiOptions.ChatModelName,
                        apiKey: openAiOptions.ApiKey)
            .WithInstructions("Provide user profile information, like City, Age, and Phone number")
            .WithName("UserProfileAgent")
            .WithDescription("An Agent that provides user profile information.")
            .WithPlugin(plugin)
            .BuildAsync();
    }
}
