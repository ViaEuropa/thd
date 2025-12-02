using System.Net;

namespace Thd.Reader;

public sealed class HttpStatusCodeNopResolver : IHttpStatusCodeResolver
{
    public HttpStatusCode? ResolveStatusCode(ReplayData replayData)
    {
        return null;
    }
}