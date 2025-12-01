namespace Thd.TestServer.Modules.Version;

public static class Routes
{
    public static WebApplication MapVersionRoutes(this WebApplication app)
    {
        app.MapGet("/version", () => new { Version = "1.0.0", Name = "Thd Test Server" })
           .WithName("GetVersion");

        return app;
    }
}