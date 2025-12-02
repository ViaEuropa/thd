using System.Net;

namespace Thd.Reader;

public interface IHttpStatusCodeResolver
{
    public HttpStatusCode? ResolveStatusCode(ReplayData replayData);
}