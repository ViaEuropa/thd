using System.Globalization;
using System.Text;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Thd.Request;

public static class UrlFilter
{
    public static string GenerateUniqueKey(Uri url)
    {
        StringBuilder builder = new();
        builder.Append(url.DnsSafeHost.ToLower(CultureInfo.InvariantCulture));

        foreach (string urlSegment in url.Segments)
        {
            if (urlSegment == "/")
            {
                continue;
            }

            builder.Append('/');

            if (RegexOptions.CouldBeAnIdentifier().IsMatch(urlSegment))
            {
                builder.Append("{id}");
            }
            else
            {
                builder.Append(urlSegment.Trim('/').ToLower(CultureInfo.InvariantCulture));
            }
        }

        Dictionary<string, StringValues> dictionary = QueryHelpers.ParseQuery(url.Query);

        foreach (string key in dictionary.Keys.Order())
        {
            builder.Append('?');
            builder.Append(key.ToLower(CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }
}