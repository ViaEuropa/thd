namespace Thd.Reader;

/// <summary>
/// Main contract for source data in order to be able to compare requests
/// </summary>
public record ReplayData
{
    /// <summary>
    /// Path and query for a request to be compared
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Data that will be available to for the template engine to build base urls e.g. column_1
    /// </summary>
    public required IDictionary<string, string> RoutingData { get; init; }
}