namespace CozyKitchen.Extensions;
public static class PathExtensions
{
    public static string GetPluginsRootFolder()
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string pluginsFolderPath = Path.Combine(currentDirectory, "../../../Plugins/Semantic");

        return Path.GetFullPath(pluginsFolderPath);
    }
}
