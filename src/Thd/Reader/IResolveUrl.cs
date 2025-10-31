using Thd.Commands.Compare;

namespace Thd.Reader;

public interface IResolveUrl
{
    public ResolvedUrl ResolveUrl(ReplayData url);
}