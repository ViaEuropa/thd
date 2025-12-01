namespace Thd.TestServer.Modules.Version;

public static class Routes
{
    public static WebApplication MapVersionRoutes(this WebApplication app)
    {
        app.MapGet("/version", () => new { Version = "1.0.0", Name = "Thd Test Server" })
           .WithName("GetVersion");

        app.MapGet("v2/version", () => new
        {
            Version = "1.0.0",
            Name = "Thd Test Server",
            Sha = "111d67bb9717ea97522ad10b2b023f54bc7e35f5"
        })
            .WithName("GetVersionV2");

        return app;
    }
}