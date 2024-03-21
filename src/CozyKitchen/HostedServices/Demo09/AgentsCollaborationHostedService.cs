using CozyKitchen.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace CozyKitchen;

public class AgentsCollaborationHostedService : IHostedService
{
    private readonly ILogger _logger;
    private readonly Kernel _kernel;

    public AgentsCollaborationHostedService(
        ILogger<AgentsCollaborationHostedService> logger,
        Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var inputMessage = @"
            Book a getaway to New York for five days next month.
            Pick any date that works for my calendar.";

        var openAiOptions = _kernel.Services.GetRequiredService<IOptions<OpenAiOptions>>()!.Value;

        var calendar = await _kernel.Services.GetRequiredService<Calendar>().GetAgent();
        var travel = await _kernel.Services.GetRequiredService<Travel>().GetAgent();
        var airport = await _kernel.Services.GetRequiredService<Airport>().GetAgent();
        var userProfile = await _kernel.Services.GetRequiredService<UserProfile>().GetAgent();

        var tripManagerAgent = await new AgentBuilder()
            .WithAzureOpenAIChatCompletion(
                        endpoint: openAiOptions.ApiEndpoint,
                        model: openAiOptions.ChatModelName,
                        apiKey: openAiOptions.ApiKey)
            .WithInstructions(@"
                A trip manager agent that can help you plan your trips and manage your calendar, travel, and airport information.
                First, you must find out the user's city, then you have to get the Airport code for the city
                After that, you can find the available flights from user's city airport to the destination city airport
                Finally, you can book the flight and create a new event in the user's calendar.
                If a decision needs to be made about available dates or flights or whatever, DO NOT ask the user, make the decision yourself.
                ")
            .WithName("TripManagerAgent")
            .WithDescription("An agent that can help you plan your trips and manage your calendar, travel, and airport information.")
            .WithPlugins([calendar.AsPlugin(), travel.AsPlugin(), airport.AsPlugin(), userProfile.AsPlugin()])
            .BuildAsync();

        var thread = await tripManagerAgent.NewThreadAsync(cancellationToken);

        await foreach (var responseMessage in thread.InvokeAsync(tripManagerAgent, inputMessage, cancellationToken: cancellationToken))
        {
            Console.WriteLine($"[{responseMessage.Id}]");
            Console.WriteLine($"# {responseMessage.Role}: {responseMessage.Content}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("HostedService Stopped");
        return Task.CompletedTask;
    }
}
