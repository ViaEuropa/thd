namespace Thd.Commands.Compare;

public sealed record CompareCommand
{
    public required FileInfo SourceFile { get; init; }

    public required CompareConfiguration Configuration { get; init; }

    public required RequestConfiguration ActualRequestConfiguration { get; init; }
    public required RequestConfiguration ExpectedReportConfiguration { get; init; }
}

public sealed record CompareConfiguration(bool IsInteractive, bool ShouldNormalize);