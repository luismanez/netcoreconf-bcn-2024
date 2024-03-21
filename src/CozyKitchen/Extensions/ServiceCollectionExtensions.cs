using CozyKitchen.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace CozyKitchen.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSemanticKernelWithChatCompletionsAndEmbeddingGeneration(
        this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var openAiOptions = serviceProvider.GetRequiredService<IOptions<OpenAiOptions>>()!.Value;

        var kernelBuilder = services.AddKernel();
        kernelBuilder.Services
            .AddAzureOpenAIChatCompletion(
                endpoint: openAiOptions.ApiEndpoint,
                deploymentName: openAiOptions.ChatModelName,
                apiKey: openAiOptions.ApiKey)
            .AddAzureOpenAITextEmbeddingGeneration(
                endpoint: openAiOptions.ApiEndpoint,
                deploymentName: openAiOptions.EmbeddingsModelName,
                apiKey: openAiOptions.ApiKey
            );

        return services;
    }
}
