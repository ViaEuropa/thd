using Thd.Reader;
using Thd.Request;

namespace Thd.Commands.Compare;

public static class CompareCommandHandler
{
    public static async Task<int> Handle(CompareCommand command, CancellationToken token)
    {
        IResolveUrl resolveExpected = new TemplateUrlResolver(command.ExpectedReportConfiguration.BaseUrlTemplate);
        IResolveUrl resolveActual = new TemplateUrlResolver(command.ActualRequestConfiguration.BaseUrlTemplate);
        GetRequest requestExpected = new GetRequest(new HttpClient(), command.ExpectedReportConfiguration);
        GetRequest requestActual = new GetRequest(new HttpClient(), command.ActualRequestConfiguration);

        IReplayDataReader reader = new CsvReader(command.SourceFile.FullName);

        var requests = reader.Read(token)
            .Select(data =>
            {
                var urlExpected = resolveExpected.ResolveUrl(data);
                var urlActual = resolveActual.ResolveUrl(data);

                return new CompareRequestData(urlActual, urlExpected);
            });

        IAsyncEnumerable<CompareRequestData> filteredRequests = FilterRequests(requests, command.Configuration);

        await foreach (var request in filteredRequests.WithCancellation(token))
        {
            await Diff.CompareRequests(command.Configuration, requestExpected, requestActual, request, token);
        }

        return 0;

    }

    private static IAsyncEnumerable<CompareRequestData> FilterRequests(IAsyncEnumerable<CompareRequestData> requests, CompareConfiguration commandConfiguration)
    {
        var filteredRequests = commandConfiguration.PathStartsWith == null
            ? requests
            : requests.Where(task => task.UrlExpected.DestinationUrl.PathAndQuery.StartsWith(commandConfiguration.PathStartsWith));

        Func<Uri, string>? distinctKey = commandConfiguration.Filter switch
        {
            Filter.None => null,
            Filter.Unique => uri => uri.ToString(),
            Filter.UniquePattern => UrlFilter.GenerateUniqueKey,
            _ => throw new ArgumentOutOfRangeException()
        };

        var distinctRequests = distinctKey == null
            ? filteredRequests
            : filteredRequests.DistinctBy(data => distinctKey(data.UrlExpected.DestinationUrl));

        return distinctRequests;
    }
}