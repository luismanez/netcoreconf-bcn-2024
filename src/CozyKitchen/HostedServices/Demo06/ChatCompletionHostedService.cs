using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace CozyKitchen.HostedServices;
public class ChatCompletionHostedService : IHostedService
{
    private readonly IChatCompletionService _chatCompletion;
    private readonly ILogger<ChatCompletionHostedService> _logger;

    public ChatCompletionHostedService(
        Kernel kernel,
        ILogger<ChatCompletionHostedService> logger)
    {
        _chatCompletion = kernel.Services.GetService<IChatCompletionService>()!;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var chat = new ChatHistory("You are an AI willing to help");
        // simulating adding chat history:
        chat.AddUserMessage("List me 3 famous songs from The Beattles");
        chat.AddAssistantMessage("Hey Jude, Let it be, Yesterday");
        // new question (based on history):
        chat.AddUserMessage("When was written the 3rd one?");

        var chatRequestSettings = new OpenAIPromptExecutionSettings()
        {
            MaxTokens = 1024,
            ResultsPerPrompt = 1,
            Temperature = 1,
            TopP = 0.5,
            FrequencyPenalty = 0,
        };

        var completions = await _chatCompletion.GetChatMessageContentAsync(chat, chatRequestSettings);
        var content = completions.Content;
        _logger.LogInformation("ChatCompletion result for 'When was written the 3rd one?'");
        _logger.LogInformation($"{content}");
        _logger.LogInformation($"Metadata: {completions.Metadata!.AsJson()}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("HostedService Stopped");
        return Task.CompletedTask;
    }
}
