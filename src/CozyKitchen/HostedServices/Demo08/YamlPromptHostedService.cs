using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace CozyKitchen;

public class YamlPromptHostedService : IHostedService
{
    private ILogger<YamlPromptHostedService> _logger;
    private readonly Kernel _kernel;

    public YamlPromptHostedService(
        Kernel kernel,
        ILogger<YamlPromptHostedService> logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var glossaryYaml = EmbeddedResource.Read("Glossary.yaml");
        var function = _kernel.CreateFunctionFromPromptYaml(glossaryYaml);

        var result = await _kernel.InvokeAsync<string>(function, arguments: new()
            {
                { "topic", "Clean Energy" },
                { "length", "7" },
            },
            cancellationToken: cancellationToken);

        _logger.LogInformation(result);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("HostedService Stopped");
        return Task.CompletedTask;
    }
}
