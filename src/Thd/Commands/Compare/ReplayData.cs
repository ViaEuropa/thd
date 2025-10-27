namespace Thd.Commands.Compare;

public record ReplayData
{
    public required string Path { get; init; }

    public required IDictionary<string, string> RoutingData { get; init; }
}