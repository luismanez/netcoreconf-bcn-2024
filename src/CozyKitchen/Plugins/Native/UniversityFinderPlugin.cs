using System.ComponentModel;
using System.Net.Http.Json;
using Microsoft.SemanticKernel;

namespace CozyKitchen.Plugins.Native;
public class UniversityFinderPlugin
{
    private readonly HttpClient _client;

    public UniversityFinderPlugin(
        HttpClient client)
    {
        _client = client;
    }

    [KernelFunction, Description("Get a list of universities in the given country")]
    public async Task<string> ListByCountry(
        [Description("Country to find universities into")]
        string country,
        [Description("Number of universities to return")]
        int top)
    {
        var endpoint = $"http://universities.hipolabs.com/search?country={country}";
        var data = await _client.GetFromJsonAsync<IEnumerable<University>>(endpoint);

        return string.Join("\n", data!.Take(top).Select(u => u.Name));
    }
}

public class University
{
    public string Name { get; set; } = "";
    public string State { get; set; } = "";
}
