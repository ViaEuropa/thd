using System.Text.Json.JsonDiffPatch;
using System.Text.Json.Nodes;

using Spectre.Console;
using Spectre.Console.Json;

using Thd.Request;

namespace Thd.Commands.Compare;

public static class Diff
{
    public static async Task CompareRequests(CompareConfiguration configuration, GetRequest requestExpected,
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


        if (expectedResult.StatusCode != actualResult.StatusCode)
        {
            Console.WriteLine($"{expectedResult.StatusCode} != {actualResult.StatusCode}");

            if (configuration.IsInteractive)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            return;
        }

        var diff = expectedResult.Node.Diff(actualResult.Node);
        if (diff == null || IsEmptyObject(diff))
        {
            Console.WriteLine("Equals: " + expectedResult.InspectionUrl.PathAndQuery);
        }
        else
        {
            Console.WriteLine($"Expected url: {expectedResult.InspectionUrl}");
            Console.WriteLine($"Actual url  : {actualResult.InspectionUrl}");

            var json = new JsonText(diff.ToString());

            AnsiConsole.Write(json);
            AnsiConsole.WriteLine();

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