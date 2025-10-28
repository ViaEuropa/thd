namespace Thd.Reader;

public interface IReplayDataReader
{
    IAsyncEnumerable<ReplayData> Read(CancellationToken token);
}