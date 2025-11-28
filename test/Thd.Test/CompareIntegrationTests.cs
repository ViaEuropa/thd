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

    private record ExecResult(int ExitCode, string StdOut);

    private async Task<ExecResult> ExecThd(string[] args)
    {
        TextWriter currentOut = Console.Out;

        try
        {
            await using StringWriter stringWriter = new();
            Console.SetOut(stringWriter);

            var exitCode = await Program.Main(args);
            return new ExecResult(exitCode, stringWriter.ToString());
        }
        finally
        {
            Console.SetOut(currentOut);
        }
    }

    private static async Task<string> CreateSourceFile(string content, CancellationToken cancellationToken)
    {
        string source = Path.GetTempFileName();
        await File.WriteAllTextAsync(source, content, cancellationToken);
        return source;
    }
}