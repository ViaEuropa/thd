using System.Text.RegularExpressions;

namespace Thd.Request;

internal static partial class RegexOptions
{
    [GeneratedRegex("""[\d]{2,}""")]
    public static partial Regex CouldBeAnIdentifier();
}