using System.Net;

using Scriban;

namespace Thd.Reader;

public sealed class HttpStatusCodeTemplateResolver : IHttpStatusCodeResolver
{
    private readonly Template _template;

    public HttpStatusCodeTemplateResolver(string template)
    {
        _template = Template.Parse(template);
    }

    public HttpStatusCode? ResolveStatusCode(ReplayData replayData)
    {
        string httpStatusCode = _template.Render(replayData.RoutingData);
        if (!int.TryParse(httpStatusCode, out int httpStatusCodeInt))
        {
            return null;
        }

        if (!Enum.IsDefined(typeof(HttpStatusCode), httpStatusCodeInt))
        {
            return null;
        }

        return (HttpStatusCode)httpStatusCodeInt;
    }
}