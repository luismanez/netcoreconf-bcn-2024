

using System.ComponentModel;
using CozyKitchen.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace CozyKitchen;

public class Airport
{
    private readonly ILogger<Airport> _logger;
    private readonly Kernel _kernel;
    private static readonly Dictionary<string, string> _airportCodes = new()
    {
        { "Valencia", "VLC" },
        { "New York", "JFK" },
        { "London", "LHR" },
        { "Paris", "CDG" },
        { "Tokyo", "HND" },
        { "Sydney", "SYD" },
        { "Dubai", "DXB" },
        { "Los Angeles", "LAX" },
        { "Shanghai", "PVG" },
        { "Beijing", "PEK" }
    };

    public Airport(ILogger<Airport> logger, Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel;
    }

    [KernelFunction]
    [Description("Get the Airport code for the specified city")]
    public string GetAirportCodeByCityName(
        [Description("Name of the city")]
        string cityName)
    {
        _logger.LogInformation("Getting airport code for {cityName}", cityName);
        return _airportCodes.GetValueOrDefault(cityName, "N/A");
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
            .WithInstructions("Provide Airport information, like Airport code for a city")
            .WithName("AirportAgent")
            .WithDescription("An Agent that provides Airport information.")
            .WithPlugin(plugin)
            .BuildAsync();
    }
}
