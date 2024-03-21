using CozyKitchen;
using CozyKitchen.Extensions;
using CozyKitchen.HostedServices;
using CozyKitchen.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        configHost.SetBasePath(currentDirectory);
        configHost.AddJsonFile("hostsettings.json", optional: false);
        configHost.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddOptions();
        services.AddOptions<OpenAiOptions>()
            .Bind(configuration.GetSection(OpenAiOptions.SettingsSectionName));

        services.AddLogging(configure => configure.AddConsole());

        services.AddSemanticKernelWithChatCompletionsAndEmbeddingGeneration();

        services.AddHttpClient();

        var demoToRun = args.Length > 0 ? args[0] : "1";

        //demoToRun = "9";

        switch (demoToRun)
        {
            default:
                services.AddHostedService<HelloSemanticWorldHostedService>();
                break;
            case "2":
                services.AddHostedService<SemanticFunctionWithParamsHostedService>();
                break;
            case "3":
                //services.AddHostedService<NativeFunctionHostedService>();
                services.AddHostedService<NestedFunctionHostedService>();
                break;
            case "4":
                services.AddHostedService<PlannerHostedService>();
                break;
            case "5":
                services.AddHostedService<EmbeddingGenerationHostedService>();
                break;
            case "6":
                services.AddHostedService<ChatCompletionHostedService>();
                break;
            case "7":
                services.AddSingleton<IFunctionFilter, DiagnosticsFunctionFilter>();
                services.AddSingleton<IFunctionFilter, DummyReplacerFunctionFilter>();
                services.AddSingleton<IPromptFilter, DiagnosticsPromptFilter>();
                services.AddHostedService<FunctionFilterHostedService>();
                break;
            case "8":
                services.AddHostedService<YamlPromptHostedService>();
                break;
            case "9":
                services.AddSingleton<Calendar>();
                services.AddSingleton<Airport>();
                services.AddSingleton<Travel>();
                services.AddSingleton<UserProfile>();
                services.AddHostedService<AgentsCollaborationHostedService>();
                break;
        }
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<IHost>>();
try
{
    host.Run();
}
catch (OptionsValidationException ex)
{
    foreach (var failure in ex.Failures)
    {
        logger!.LogError(failure);
    }
}