using JetBrains.Annotations;

using Microsoft.AspNetCore.Builder;

using Thd.TestServer;

namespace Thd;

[UsedImplicitly]
public class ApiFixture : IAsyncLifetime
{
    private readonly WebApplication _application;

    public ApiFixture()
    {
        _application = Server.CreateWebApplication([]);
    }

    public string BasePath => "http://localhost:5000";

    public async ValueTask DisposeAsync()
    {
        await _application.StopAsync();
        await _application.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _application.StartAsync();
    }
}