using System.Net;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;

using Scriban;

namespace Thd.Commands.Compare;

public static class Diff
{
    public static async Task CompareRequests(CompareConfiguration configuration, GetRequest originConfiguration,
        GetRequest destinationConfiguration,
        ReplayData url, CancellationToken cancellationToken)
    {
        RequestResult originResult = await originConfiguration.Get(url, configuration.ShouldNormalize, cancellationToken);
        RequestResult destinationResult = await destinationConfiguration.Get(url, configuration.ShouldNormalize, cancellationToken);


        if (originResult.StatusCode != destinationResult.StatusCode)
        {
            Console.WriteLine($"{originResult.StatusCode} != {destinationResult.StatusCode}");

            if (configuration.IsInteractive)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            return;
        }

        var diff = originResult.Node.Diff(destinationResult.Node);
        if (IsEmptyObject(diff))
        {
            Console.WriteLine("No differences found for " + url);
        }
        else
        {
            Console.WriteLine(diff);

            if (configuration.IsInteractive)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private static bool IsEmptyObject(JsonNode? node)
    {
        if (node == null)
        {
            return true;
        }

        return node is JsonObject { Count: 0 };
    }
}

public interface IResolveUrl
{
    public Task<TemplateUrlResolver.ResolvedUrl> ResolveUrl(ReplayData url);
}

public sealed class TemplateUrlResolver : IResolveUrl
{
    private readonly Template _template;

    public TemplateUrlResolver(string template)
    {
        _template = Template.Parse(template);
    }

    public async Task<ResolvedUrl> ResolveUrl(ReplayData url)
    {
        string baseUrl = await ParseBaseUrl(_template, url);

        var destination = new Uri(baseUrl + url.Path);

        return new ResolvedUrl(baseUrl, destination);
    }

    public record ResolvedUrl(string BaseUrl, Uri DestinationUrl);

    private static async Task<string> ParseBaseUrl(Template template, ReplayData url)
    {
        var rendered = await template.RenderAsync(url.RoutingData);
        return rendered;
    }
}

public class GetRequest
{
    private readonly HttpClient _httpClient;
    private readonly RequestConfiguration _requestConfiguration;
    private readonly IResolveUrl _urlResolver;

    public GetRequest(HttpClient httpClient, RequestConfiguration requestConfiguration)
    {
        _requestConfiguration = requestConfiguration ?? throw new ArgumentNullException(nameof(requestConfiguration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _urlResolver = new TemplateUrlResolver(requestConfiguration.BaseUrlTemplate);
    }

    public async Task<RequestResult> Get(ReplayData url, bool shouldNormalizeResponse, CancellationToken token)
    {
        TemplateUrlResolver.ResolvedUrl destinationUri = await _urlResolver.ResolveUrl(url);

        Console.WriteLine("Calling " + destinationUri);
        HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, destinationUri.DestinationUrl);
        if (!string.IsNullOrWhiteSpace(_requestConfiguration.ApiKey))
        {
            httpRequestMessage.Headers.Add("Authorization", _requestConfiguration.ApiKey);
        }
        var response =
            await _httpClient.SendAsync(httpRequestMessage, token);

        if (!response.IsSuccessStatusCode)
        {
            return new RequestResult(null, response.StatusCode);
        }

        string responseContent = await response.Content.ReadAsStringAsync(token);

        var json = shouldNormalizeResponse
            ? responseContent.Replace(destinationUri.BaseUrl, "")
            : responseContent;

        var node = JsonNode.Parse(json);
        return new RequestResult(node, response.StatusCode);
    }
}

public record RequestResult(JsonNode? Node, HttpStatusCode StatusCode);