using System.Net;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;

using Spectre.Console;
using Spectre.Console.Json;

using Thd.Reader;
using Thd.Request;

namespace Thd.Commands.Compare;

public static class Diff
{
    public static async Task CompareRequests(CompareConfiguration configuration,
        GetRequest requestExpected,
        GetRequest requestActual,
        CompareRequestData url, CancellationToken cancellationToken)
    {
        ResponseConfiguration responseConfiguration = new()
        {
            ShouldNormalizeBaseUrl = configuration.ShouldNormalizeBaseUrlInResponse,
            ShouldUpgradeHttpToHttps = configuration.UpgradeHttpToHttpInResponse
        };

        RequestResult expectedResult =
            await requestExpected.Get(url.UrlExpected, responseConfiguration, cancellationToken);
        RequestResult actualResult =
            await requestActual.Get(url.UrlActual, responseConfiguration, cancellationToken);

        // Determine expected status code
        // 1. From the --expected-http-status option
        // 2. From the expected request result
        // 3. Default to 200 OK
        HttpStatusCode expectedStatusCode = url.ExpectedStatusCode ?? expectedResult.StatusCode ?? HttpStatusCode.OK;

        await RenderResult(AnsiConsole.Console, configuration, expectedResult, actualResult, expectedStatusCode, cancellationToken);
    }

    private static async Task RenderResult(IAnsiConsole console, CompareConfiguration configuration,
        RequestResult expectedResult,
        RequestResult actualResult, HttpStatusCode expectedStatusCode, CancellationToken cancellationToken)
    {
        bool identicalRequests = IsIdenticalRequests(expectedStatusCode, expectedResult, actualResult);

        if (identicalRequests)
        {
            console.MarkupLineInterpolated(
                $"{Emoji.Known.CheckMarkButton} {expectedResult.InspectionUrl.PathAndQuery}");
        }
        else
        {
            console.MarkupLineInterpolated($"{Emoji.Known.CrossMark} {expectedResult.InspectionUrl.PathAndQuery}");
        }

        // Detailed rendering of the requests
        if (!identicalRequests || configuration.Verbosity.IsAtLeastDetailed())
        {
            RenderRequestData(console, expectedResult, "Expected");
            RenderRequestData(console, actualResult, "Actual");
        }

        if (expectedStatusCode != expectedResult.StatusCode || expectedStatusCode != actualResult.StatusCode)
        {
            console.Write("  HTTP status diff:");
            if (expectedStatusCode != expectedResult.StatusCode)
            {
                console.Write($" (Expected {expectedResult.StatusCode} !== {expectedStatusCode})");
            }
            if (expectedStatusCode != actualResult.StatusCode)
            {
                console.Write($" (Actual {actualResult.StatusCode} !== {expectedStatusCode})");
            }
            console.WriteLine();
        }

        var diff = expectedResult.Node.Diff(actualResult.Node);
        if (diff != null && !IsEmptyObject(diff))
        {
            console.WriteLine("  JSON diff:");

            var json = new JsonText(diff.ToString());
            json.BracesColor(Color.Blue);
            json.BracketColor(Color.Blue);
            json.CommaColor(Color.Blue);
            json.NullColor(Color.Blue);

            console.Write(json);
            console.WriteLine();
        }

        if (configuration.IsInteractive && !identicalRequests)
        {
            console.WriteLine("Press any key to continue...");
            await console.Input.ReadKeyAsync(true, cancellationToken);
        }
    }

    private static void RenderRequestData(IAnsiConsole console, RequestResult expectedResult, string title)
    {
        console.Write($"  {title} (Status {expectedResult.StatusCode?.ToString() ?? "N/A"})");
        if (expectedResult.Error != null)
        {
            console.Write(" - Error: " + Enum.GetName(typeof(HttpRequestError), expectedResult.Error));
        }

        console.WriteLine();
        console.MarkupLineInterpolated($"    [link]{expectedResult.InspectionUrl}[/]");
    }

    private static bool IsIdenticalRequests(HttpStatusCode expectedStatusCode, RequestResult expectedResult,
        RequestResult actualResult)
    {
        // We cannot consider requests as identical if one of them has an error
        if (expectedResult.Error != null || actualResult.Error != null)
        {
            return false;
        }

        if (expectedStatusCode != expectedResult.StatusCode || expectedStatusCode != actualResult.StatusCode)
        {
            return false;
        }

        var diff = expectedResult.Node.Diff(actualResult.Node);
        return diff == null || IsEmptyObject(diff);
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