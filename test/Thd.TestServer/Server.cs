using Thd.TestServer.Modules.Version;

namespace Thd.TestServer;

public static class Server
{
    public static WebApplication CreateWebApplication(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        WebApplication app = builder.Build();

        app.MapVersionRoutes();

        return app;
    }
}