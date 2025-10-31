using System.Text.Json.Nodes;

using Thd.Commands.Compare;

namespace Thd.Request;

public class GetRequest
{
    private readonly HttpClient _httpClient;
    private readonly RequestConfiguration _requestConfiguration;

    public GetRequest(HttpClient httpClient, RequestConfiguration requestConfiguration)
    {
        _requestConfiguration = requestConfiguration ?? throw new ArgumentNullException(nameof(requestConfiguration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<RequestResult> Get(ResolvedUrl url, ResponseConfiguration responseConfiguration,
        CancellationToken token)
    {
        HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, url.DestinationUrl);
        if (!string.IsNullOrWhiteSpace(_requestConfiguration.ApiKey))
        {
            httpRequestMessage.Headers.Add("Authorization", _requestConfiguration.ApiKey);
        }

        var response =
            await _httpClient.SendAsync(httpRequestMessage, token);

        if (!response.IsSuccessStatusCode)
        {
            return new RequestResult(url.DestinationUrl, null, response.StatusCode);
        }

        string responseContent = await response.Content.ReadAsStringAsync(token);

        var normalizedResponse = responseConfiguration.ShouldUpgradeHttpToHttps
            ? responseContent.Replace("http://", "https://")
            : responseContent;

        normalizedResponse = responseConfiguration.ShouldNormalizeBaseUrl
            ? normalizedResponse
                .Replace(url.BaseUrl, "")
            : responseContent;

        var mediaType = response.Content.Headers.ContentType?.MediaType;

        var json = WrapResponse(mediaType, normalizedResponse);

        var node = JsonNode.Parse(json);

        return new RequestResult(url.DestinationUrl, node, response.StatusCode);
    }

    private string WrapResponse(string? mediaType, string response)
    {
        if (mediaType == null)
        {
            return """{ "value": null }""";
        }

        if (mediaType == "text/plain")
        {
            return $$$"""{ "value": "{{{response}}}" }""";
        }

        if (mediaType.Contains("json"))
        {
            return response;
        }

        return $$$"""{ "value": "{{{mediaType}}}" }""";
    }
}