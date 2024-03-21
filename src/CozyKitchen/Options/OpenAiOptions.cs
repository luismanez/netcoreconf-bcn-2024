namespace CozyKitchen.Options;
public class OpenAiOptions
{
    public const string SettingsSectionName = "OpenAi";
    public string ApiEndpoint { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string ChatModelName { get; set; } = "";
    public string EmbeddingsModelName { get; set; } = "";
}
