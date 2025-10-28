namespace Thd.Request;

public record RequestConfiguration
{
    /// <summary>
    /// Scriban template for the base URL.
    /// https://github.com/scriban/scriban
    /// </summary>
    public required string BaseUrlTemplate { get; init; }

    /// <summary>
    /// Optional Authorization Header if needed for the requests
    /// </summary>
    public required string? ApiKey { get; init; }
}