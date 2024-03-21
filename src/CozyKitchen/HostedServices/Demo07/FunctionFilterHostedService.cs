using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace CozyKitchen;

public class FunctionFilterHostedService : IHostedService
{
    private readonly Kernel _kernel;
    private readonly ILogger<FunctionFilterHostedService> _logger;

    public FunctionFilterHostedService(Kernel kernel, ILogger<FunctionFilterHostedService> logger)
    {
        _logger = logger;
        _kernel = kernel;

    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // filters have been added using DI in program.cs

        // to add firlters without DI:
        // _kernel.PromptFilters.Add(new DiagnosticsPromptFilter(_logger));
        // _kernel.FunctionFilters.Add(new DiagnosticsFunctionFilter(_logger));

        var function = _kernel.CreateFunctionFromPrompt(
            "What is Azure",
            functionName: "MyFunction");

        var result = await _kernel.InvokeAsync(function);

        _logger.LogInformation($"Result: {result.GetValue<string>()}. Metadata: {result.Metadata!.AsJson()}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("HostedService Stopped");
        return Task.CompletedTask;
    }
}

public sealed class DiagnosticsFunctionFilter : IFunctionFilter
{
    private readonly ILogger<DiagnosticsFunctionFilter> _logger;

    public DiagnosticsFunctionFilter(ILogger<DiagnosticsFunctionFilter> logger)
    {
        _logger = logger;
    }

    public void OnFunctionInvoking(FunctionInvokingContext context) =>
        _logger.LogInformation($"{nameof(DiagnosticsFunctionFilter)}.{nameof(OnFunctionInvoking)} - {context.Function.Name}.");

    public void OnFunctionInvoked(FunctionInvokedContext context) =>
        _logger.LogInformation($"{nameof(DiagnosticsFunctionFilter)}.{nameof(OnFunctionInvoked)} - {context.Function.Name}.");
}

public sealed class DummyReplacerFunctionFilter : IFunctionFilter
{
    private readonly ILogger<DummyReplacerFunctionFilter> _logger;

    public DummyReplacerFunctionFilter(ILogger<DummyReplacerFunctionFilter> logger)
    {
        _logger = logger;
    }

    public void OnFunctionInvoking(FunctionInvokingContext context) =>
        _logger.LogInformation($"{nameof(DummyReplacerFunctionFilter)}.{nameof(OnFunctionInvoking)} - {context.Function.Name}.");

    public void OnFunctionInvoked(FunctionInvokedContext context)
    {
        _logger.LogInformation($"{nameof(DummyReplacerFunctionFilter)}.{nameof(OnFunctionInvoked)} - {context.Function.Name}.");
        var originalResult = context.Result;
        var newResult = originalResult.GetValue<string>()!.Replace("Azure", "MICROSOFT AZURE");
        context.SetResultValue(newResult);
    }
}

public sealed class DiagnosticsPromptFilter : IPromptFilter
{
    private readonly ILogger<DiagnosticsPromptFilter> _logger;

    public DiagnosticsPromptFilter(ILogger<DiagnosticsPromptFilter> logger)
    {
        _logger = logger;
    }

    public void OnPromptRendering(PromptRenderingContext context)
    {
        _logger.LogInformation(
            $"{nameof(DiagnosticsPromptFilter)}.{nameof(OnPromptRendering)}");
    }

    public void OnPromptRendered(PromptRenderedContext context)
    {
        _logger.LogInformation(
            $"{nameof(DiagnosticsPromptFilter)}.{nameof(OnPromptRendered)}. Prompt: {context.RenderedPrompt}");
    }
}
