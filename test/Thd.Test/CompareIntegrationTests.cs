using Spectre.Console;
using Spectre.Console.Testing;

namespace Thd;

[Collection("Api collection")]
public class CompareIntegrationTests
{
    private readonly ApiFixture _apiFixture;

    public CompareIntegrationTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Compare_HappyFlow()
    {
        const string content = """
                               Id;Path
                               1;/version
                               """;

        string sourceFile = await CreateSourceFile(content, TestContext.Current.CancellationToken);

        string[] args =
        [
            "compare",
            "-l",
            _apiFixture.BasePath,
            "-r",
            _apiFixture.BasePath,
            sourceFile
        ];

        ExecResult result = await ExecThd(args);

        Assert.Equal(0, result.ExitCode);
        await Verify(result.StdOut);
    }

    [Fact]
    public async Task Compare_StatusCodeDiff()
    {
        const string content = """
                               Id;Path
                               1;/version
                               """;

        string sourceFile = await CreateSourceFile(content, TestContext.Current.CancellationToken);

        string[] args =
        [
            "compare",
            "-l",
            _apiFixture.BasePath,
            "-r",
            _apiFixture.BasePath + "/notfound",
            sourceFile
        ];

        ExecResult result = await ExecThd(args);

        Assert.Equal(0, result.ExitCode);
        await Verify(result.StdOut);
    }

    [Fact]
    public async Task Compare_NoServer()
    {
        const string content = """
                               Id;Path
                               1;/version
                               """;

        string sourceFile = await CreateSourceFile(content, TestContext.Current.CancellationToken);

        string[] args =
        [
            "compare",
            "-l",
            _apiFixture.BasePath,
            "-r",
            "https://localhost:1567",
            sourceFile
        ];

        ExecResult result = await ExecThd(args);

        await Verify(result.StdOut);
        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public async Task Compare_JsonDiff()
    {
        const string content = """
                               Id;Path
                               1;/version
                               """;

        string sourceFile = await CreateSourceFile(content, TestContext.Current.CancellationToken);

        string[] args =
        [
            "compare",
            "-l",
            _apiFixture.BasePath,
            "-r",
            $"{_apiFixture.BasePath}/v2",
            sourceFile
        ];

        ExecResult result = await ExecThd(args);

        await Verify(result.StdOut);
        Assert.Equal(0, result.ExitCode);
    }

    private record ExecResult(int ExitCode, string StdOut);

    private async Task<ExecResult> ExecThd(string[] args)
    {
        using TestConsole testConsole = new();

        // NOTE: this is not optimal, but since we're testing Main in a single collection it should be fine for now.
        AnsiConsole.Console = testConsole;

        var exitCode = await Program.Main(args);
        return new ExecResult(exitCode, testConsole.Output);
    }

    private static async Task<string> CreateSourceFile(string content, CancellationToken cancellationToken)
    {
        string source = Path.GetTempFileName();
        await File.WriteAllTextAsync(source, content, cancellationToken);
        return source;
    }
}