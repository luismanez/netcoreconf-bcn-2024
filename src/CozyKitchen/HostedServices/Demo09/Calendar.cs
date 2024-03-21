using System.ComponentModel;
using System.Text.Json;
using CozyKitchen.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Experimental.Agents;

namespace CozyKitchen;



public class Calendar
{
    private static readonly DateTime Now = DateTime.Now;
    private static readonly DateTime NextMonth = new(Now.Year, DateTime.Now.AddMonths(1).Month, 1);
    private static readonly int TargetMonth = NextMonth.Month;
    private static readonly int TargetYear = NextMonth.Year;
    private readonly List<CalendarEvent> _events = [];
    private readonly ILogger _logger;
    private readonly Kernel _kernel;

    private List<CalendarEvent> Events
    {
        get
        {
            if (_events.Count == 0)
            {
                _events.Add(new CalendarEvent($"{TargetMonth}-1-{TargetYear}", "Dinner with Fred", "Just hanging out"));
                _events.Add(new CalendarEvent($"{TargetMonth}-11-{TargetYear} to {TargetMonth}-13-{TargetYear}", "Work trip", "Important client"));
                _events.Add(new CalendarEvent($"{TargetMonth}-17-{TargetYear}", "Doctor"));
                _events.Add(new CalendarEvent($"{TargetMonth}-19-{TargetYear}", "Dentist"));
            }

            return _events;
        }
    }

    public Calendar(ILogger<Calendar> logger, Kernel kernel)
    {
        _logger = logger;
        _kernel = kernel;
    }

    [KernelFunction]
    [Description("Get the current date")]
    public string GetCurrentDate()
    {
        _logger.LogInformation("Getting current date");
        return DateTime.Now.Date.ToLongDateString();
    }

    [KernelFunction]
    [Description("Get list of scheduled events within the specified date range.")]
    public string GetEvents(
        [Description("The first date in the range")]
        string from,
        [Description("The last date in the range")]
        string to)
    {
        _logger.LogInformation("Getting events from {from} to {to}", from, to);
        var fromDate = DateTime.Parse(from);
        var toDate = DateTime.Parse(to);
        var result = (fromDate > NextMonth.AddMonths(1) || toDate < NextMonth) ? [] : Events;

        return JsonSerializer.Serialize(result);
    }

    [KernelFunction]
    [Description("Create a new scheduled event.")]
    public void NewEvent(string when, string title, string? description = null)
    {
        _logger.LogInformation("Creating new event for {when} with title {title}", when, title);
        Events.Add(new CalendarEvent(when, title, description));
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
            .WithInstructions("Provide user calendar information")
            .WithName("CalendarAgent")
            .WithDescription("An Agent that provides and manages user calendar information.")
            .WithPlugin(plugin)
            .BuildAsync();
    }

    private record CalendarEvent(
        string Date,
        string Title,
        string? Description = null);
}
