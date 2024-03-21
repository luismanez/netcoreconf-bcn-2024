using System.ComponentModel;
using System.Text.Json;
using CozyKitchen.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace CozyKitchen;

public class Travel
{
    public Travel(ILogger<Travel> logger, Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel;
    }

    private readonly Dictionary<string, IList<FlightListing>> _events = [];
    private readonly ILogger<Travel> _logger;
    private readonly Kernel _kernel;

    [KernelFunction]
    [Description("Search for available flights and provide list of flight numbers for a given date.")]
    public string ListFlights(
        [Description("The airport code of the departing flight.")]
        string from,
        [Description("The airport code of the arriving flight.")]
        string to,
        [Description("The date of the flight.")]
        string when)
    {
        _logger.LogInformation("Listing flights from {from} to {to} on {when}", from, to, when);

        var listings = GetFlightListings(from, to, when);

        return JsonSerializer.Serialize(listings);
    }

    [KernelFunction]
    [Description("Book the given flight by flight number.")]
    public void BookFlight(string flightNumber)
    {
        _logger.LogInformation("Booking flight {flightNumber}", flightNumber);
    }

    public async Task<IAgent> GetAgent()
    {
        var openAiOptions = _kernel.Services.GetRequiredService<IOptions<OpenAiOptions>>()!.Value;

        var plugin = KernelPluginFactory.CreateFromObject(this);

        return await new AgentBuilder()
            .WithAzureOpenAIChatCompletion(
                        endpoint: openAiOptions.ApiEndpoint,
                        model: openAiOptions.ChatModelName,
                        apiKey: openAiOptions.ApiKey)
            .WithInstructions(@"
                You are a travel agent that can list and book airline flights only when both origin and destination are explicitly defined.
                Don't forget to consider return flights")
            .WithName("TravelAgent")
            .WithDescription("An agent that provides information on available airline flights and also can book flights.  This agent does not know the location of your home and must always be told both the origin and destination.")
            .WithPlugin(plugin)
            .BuildAsync();
    }

    private IList<FlightListing> GetFlightListings(string from, string to, string when)
    {
        var key = $"{from}-{to}-{when}";

        if (!_events.TryGetValue(key, out var listings))
        {
            listings = GenerateFlightListings(from, to, when).ToArray();
            _events[key] = listings;
        }

        return listings;
    }
    private IEnumerable<FlightListing> GenerateFlightListings(string from, string to, string when)
    {
        yield return new FlightListing(Guid.NewGuid().ToString(), MakeFlightNumber(), from, "PDX", when, "$99.99");
        yield return new FlightListing(Guid.NewGuid().ToString(), MakeFlightNumber(), from, "YVR", when, "72.00");
        yield return new FlightListing(Guid.NewGuid().ToString(), MakeFlightNumber(), from, to, when, "$129.99");
        yield return new FlightListing(Guid.NewGuid().ToString(), MakeFlightNumber(), from, to, when, "$92.75");

        static string MakeFlightNumber() => $"{(char)('A' + Random.Shared.Next(26))}{(char)('A' + Random.Shared.Next(26))}{Random.Shared.Next(10)}{Random.Shared.Next(10)}{Random.Shared.Next(10)}";
    }

    private record FlightListing(
        string Id,
        string FlightNumber,
        string From,
        string To,
        string When,
        string Price,
        string Description = "one-way");
}
