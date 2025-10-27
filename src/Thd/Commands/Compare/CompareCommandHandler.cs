using nietras.SeparatedValues;

namespace Thd.Commands.Compare;

public static class CompareCommandHandler
{
    public static async Task<int> Handle(CompareCommand command, CancellationToken token)
    {
        var origin = new GetRequest(new HttpClient(), command.ActualRequestConfiguration);
        var destination = new GetRequest(new HttpClient(), command.ExpectedReportConfiguration);

        using var reader = await Sep.Reader(configure: options => options with
        {
            Unescape = true
        }).FromFileAsync(command.SourceFile.FullName, token);

        int numberOfMetaDataColumns = reader.Header.ColNames.Count - 1;

        await foreach (var readRow in reader)
        {
            ReplayData replayData = ReplayData(numberOfMetaDataColumns, readRow);

            await Diff.CompareRequests(command.Configuration, origin, destination, replayData, token);
        }

        return 0;
    }

    private static ReplayData ReplayData(int numberOfMetaDataColumns, SepReader.Row readRow)
    {
        IDictionary<string, string> routingData = new Dictionary<string, string>();
        for (int i = 0; i < numberOfMetaDataColumns; i++)
        {
            routingData["column_" + (i + 1)] = readRow[i].Span.ToString();
        }

        var replayData = new ReplayData
        {
            Path = readRow[numberOfMetaDataColumns].Span.ToString(),
            RoutingData = routingData
        };

        return replayData;
    }
}