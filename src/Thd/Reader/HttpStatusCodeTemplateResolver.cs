using System.Net;

using Scriban;

namespace Thd.Reader;

public sealed class HttpStatusCodeTemplateResolver : IHttpStatusCodeResolver
{
    private readonly Template _template;

    public HttpStatusCodeTemplateResolver(string template)
    {
        _template = Template.Parse(template); ;
    }

    public HttpStatusCode? ResolveStatusCode(ReplayData replayData)
    {
        string httpStatusCode = _template.Render(replayData.RoutingData);
        return (HttpStatusCode?)int.Parse(httpStatusCode);
    }
}