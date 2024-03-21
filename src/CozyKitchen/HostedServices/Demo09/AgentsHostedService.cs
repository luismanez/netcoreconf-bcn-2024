using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace CozyKitchen;

public class AgentsHostedService : IHostedService
{
    private readonly ILogger _logger;
    private readonly Kernel _kernel;

    public AgentsHostedService(Kernel kernel,
                                 ILogger<AgentsHostedService> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var fitnessTrainer = new ChatCompletionAgent(
           _kernel,
           instructions: "As a fitness trainer, suggest workout routines, and exercises for beginners. " +
           "You are not a stress management expert, so refrain from recommending stress management strategies. " +
           "Collaborate with the stress management expert to create a holistic wellness plan." +
           "Always incorporate stress reduction techniques provided by the stress management expert into the fitness plan." +
           "Always include your role at the beginning of each response, such as 'As a fitness trainer."
        );

        var stressManagementExpert = new ChatCompletionAgent(
            _kernel,
            instructions: "As a stress management expert, provide guidance on stress reduction strategies. " +
            "Collaborate with the fitness trainer to create a simple and holistic wellness plan." +
            "You are not a fitness expert; therefore, avoid recommending fitness exercises." +
            "If the plan is not aligned with recommended stress reduction plan, ask the fitness trainer to rework it to incorporate recommended stress reduction techniques. " +
            "Only you can stop the conversation by saying WELLNESS_PLAN_COMPLETE if suggested fitness plan is good." +
            "Always include your role at the beginning of each response such as 'As a stress management expert."
         );

        var chat = new TurnBasedChat([fitnessTrainer, stressManagementExpert], (chatHistory, replies, turn) =>
            turn >= 10 || // Limit the number of turns to 10
            replies.Any(
                message => message.Role == AuthorRole.Assistant &&
                message.Content!.Contains("WELLNESS_PLAN_COMPLETE", StringComparison.InvariantCulture))); // Exit when the message "WELLNESS_PLAN_COMPLETE" received from agent

        var prompt = "I need help creating a simple wellness plan for a beginner. Please guide me.";
        PrintConversation(await chat.SendMessageAsync(prompt, cancellationToken));
    }

    private string PrintPrompt(string prompt)
    {
        Console.WriteLine($"Prompt: {prompt}");

        return prompt;
    }

    private void PrintConversation(IEnumerable<ChatMessageContent> messages)
    {
        foreach (var message in messages)
        {
            Console.WriteLine($"------------------------------- {message.Role} ------------------------------");
            Console.WriteLine(message.Content);
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("HostedService Stopped");
        return Task.CompletedTask;
    }
}

public sealed class TurnBasedChat
{
    public TurnBasedChat(IEnumerable<ChatCompletionAgent> agents, Func<ChatHistory, IEnumerable<ChatMessageContent>, int, bool> exitCondition)
    {
        _agents = agents.ToArray();
        _exitCondition = exitCondition;
    }

    public async Task<IReadOnlyList<ChatMessageContent>> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        var chat = new ChatHistory();
        chat.AddUserMessage(message);

        IReadOnlyList<ChatMessageContent> result = new List<ChatMessageContent>();

        var turn = 0;

        do
        {
            var agent = _agents[turn % _agents.Length];

            result = await agent.InvokeAsync(chat, cancellationToken);

            chat.AddRange(result);

            turn++;
        }
        while (!_exitCondition(chat, result, turn));

        return chat;
    }

    private readonly ChatCompletionAgent[] _agents;
    private readonly Func<ChatHistory, IEnumerable<ChatMessageContent>, int, bool> _exitCondition;
}