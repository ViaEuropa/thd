namespace Thd.Request;

public record ResponseConfiguration
{
    /// <summary>
    /// Will remove the base url from the returned response
    /// </summary>
    public bool ShouldNormalizeBaseUrl { get; init; }

    /// <summary>
    /// Some applications might return http instead of https in the response that will introduce a diff
    /// </summary>
    public bool ShouldUpgradeHttpToHttps { get; init; }
}