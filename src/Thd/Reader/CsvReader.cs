using System.Runtime.CompilerServices;

using nietras.SeparatedValues;

namespace Thd.Reader;

/// <summary>
/// Will read replay data from a CSV file
/// The path needs to be located as the last column
/// </summary>
public class CsvReader : IReplayDataReader
{
    private readonly string _csvFilePath;

    public CsvReader(string csvFilePath)
    {
        _csvFilePath = csvFilePath;
    }

    public async IAsyncEnumerable<ReplayData> Read([EnumeratorCancellation] CancellationToken token)
    {

        using var reader = await Sep.Reader(configure: options => options with
        {
            Unescape = true
        }).FromFileAsync(_csvFilePath, token);

        int numberOfMetaDataColumns = reader.Header.ColNames.Count - 1;

        foreach (var readRow in reader)
        {
            yield return ReplayData(numberOfMetaDataColumns, readRow);
        }
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