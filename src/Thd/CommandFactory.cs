using System.CommandLine;

using Thd.Commands.Compare;
using Thd.Request;

namespace Thd;

public static class CommandFactory
{
    private static readonly Option<string> ExpectedBaseUrl = new("--expected-base-url", "-l")
    {
        HelpName = "https://{{ column_1 }}.api.my.tld",
        Required = true
    };

    private static readonly Option<string> ActualBaseUrl = new("--actual-base-url", "-r")
    {
        Description =
            "The base URL template for the actual requests. Use {{ column_n }} to reference metadata columns.",
        HelpName = "https://{{ column_1 }}.api.dev.my.tld/v2",
        Required = true
    };

    private static readonly Option<bool> NormalizeBaseUrl = new("--normalize-base-url", "-n")
    {
        Description = "When set, the base URL will be removed from the json response before comparison.",
        Required = false,
        DefaultValueFactory = _ => false
    };

    private static readonly Option<bool> Interactive = new("--interactive", "-i")
    {
        Description = "When set, the command might halt waiting for user input.",
        Required = false,
        DefaultValueFactory = _ => false
    };

    private static readonly Option<string?> ActualAuthorizationHeader = new("--actual-authorization-header")
    {
        Description = "Authorization header for --actual-base-url. Will read from env.THD_ACTUAL_AUTHORIZATION_HEADER if not specified.",
    };

    private static readonly Option<string?> ExpectedAuthorizationHeader = new("--expected-authorization-header")
    {
        Description = "Authorization header for --expected-base-url. Will read from env.THD_EXPECTED_AUTHORIZATION_HEADER if not specified.",
    };

    private static readonly Option<string?> PathShouldStartsWith = new("--experimental-path")
    {
        Description = "Drop paths not starting with --experimental-path.",
    };

    private static readonly Option<bool> AggressiveFiltering = new("--experimental-aggressive-filtering")
    {
        Description = "Will find unique patterns in the actual domain/path?query",
        Required = false,
        DefaultValueFactory = _ => false
    };

    private static readonly Option<bool> UpgradeHttpToHttpInResponse = new("--experimental-upgrade-http")
    {
        Description = "Will upgrade http to https for the actual responses",
        Required = false,
        DefaultValueFactory = _ => false
    };

    private static readonly Argument<FileInfo> SourceFile = new("source-file")
    {
        Description = "The file containing the list of URLs to compare."
    };

    private static Command CreateCompareCommand()
    {
        Command compareCommand =
            new("compare", "Sends GET requests to two different configurations and compares the JSON responses.")
            {
                SourceFile,
                ExpectedBaseUrl,
                ActualBaseUrl,
                ExpectedAuthorizationHeader,
                ActualAuthorizationHeader,
                NormalizeBaseUrl,
                Interactive,
                UpgradeHttpToHttpInResponse,
                PathShouldStartsWith,
                AggressiveFiltering
            };

        compareCommand.SetAction(async (result, token) =>
        {
            FileInfo file = result.GetRequiredValue(SourceFile);
            CompareCommand command = new()
            {
                ActualRequestConfiguration =
                    new RequestConfiguration
                    {
                        BaseUrlTemplate = result.GetRequiredValue(ActualBaseUrl),
                        ApiKey = result.ResolveValue(ActualAuthorizationHeader, "THD_ACTUAL_AUTHORIZATION_HEADER")
                    },
                ExpectedReportConfiguration = new RequestConfiguration
                {
                    BaseUrlTemplate = result.GetRequiredValue(ExpectedBaseUrl),
                    ApiKey = result.ResolveValue(ExpectedAuthorizationHeader, "THD_EXPECTED_AUTHORIZATION_HEADER")
                },
                Configuration =
                    new CompareConfiguration(
                        IsInteractive: result.GetValue(Interactive),
                        ShouldNormalizeBaseUrlInResponse: result.GetValue(NormalizeBaseUrl),
                        Filter: result.GetValue(AggressiveFiltering) ? Filter.UniquePattern : Filter.None,
                        PathStartsWith: result.GetValue(PathShouldStartsWith),
                        UpgradeHttpToHttpInResponse: result.GetValue(UpgradeHttpToHttpInResponse)
                    ),
                SourceFile = file
            };
            return await CompareCommandHandler.Handle(command, token);
        });

        return compareCommand;
    }

    /// <summary>
    /// Resolves the value in the following order:
    /// - --my-option
    /// - ENVIRONMENT_VARIABLE
    /// NOTE: We can't use the default value for the option, since it will be evaluated at help and printed in the -h option.
    /// </summary>
    /// <param name="result">Parse result</param>
    /// <param name="option">Preferred option to get the value from</param>
    /// <param name="environmentVariable">Name of the fallback environment variable</param>
    /// <returns>First value that's present from: cli, environment variable</returns>
    private static string? ResolveValue(this ParseResult result, Option<string?> option, string environmentVariable)
    {
        string? cliValue = result.GetValue(option);
        string? resolveValue = cliValue ?? Environment.GetEnvironmentVariable(environmentVariable);

        return resolveValue;
    }

    public static RootCommand CreateRootCommand()
    {
        RootCommand rootCommand =
            new("thd - A tool to compare HTTP GET requests between two configurations.") { CreateCompareCommand() };

        return rootCommand;
    }
}