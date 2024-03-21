using CozyKitchen.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace CozyKitchen.HostedServices;
public class SemanticFunctionWithParamsHostedService : IHostedService
{
    private readonly Kernel _kernel;
    private KernelPlugin _prompts;
    private readonly ILogger _logger;

    public SemanticFunctionWithParamsHostedService(
        Kernel kernel,
        ILogger<SemanticFunctionWithParamsHostedService> logger)
    {
        _kernel = kernel;
        _logger = logger;
        _prompts = _kernel.CreatePluginFromPromptDirectory(
            $"{PathExtensions.GetPluginsRootFolder()}/ResumeAssistantPlugin");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var chatResult = _kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
                _prompts["AboutMe"],
                new()
                {
                    { "FullName", "John Doe" },
                    { "JobTitle", "Software Engineer" },
                    { "TotalYearsOfExperience", "20" },
                    { "MainArea", "Microsoft Technologies"}
                }
            );

        string message = "";
        await foreach (var chunk in chatResult)
        {
            message += chunk;
            Console.Write(chunk);
        }
        Console.WriteLine();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("HostedService Stopped");
        return Task.CompletedTask;
    }
}