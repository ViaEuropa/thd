using System.CommandLine;

namespace Thd;

public sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        RootCommand command = CommandFactory.CreateRootCommand();
        ParseResult parseResult = command.Parse(args);

        int responseCode = await parseResult.InvokeAsync();
        return responseCode;
    }
}