using Microsoft.AspNetCore.Server.Kestrel.Core;
using Training.Training;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Microsoft.AspNetCore.Server.Kestrel.Experimental.DisableHttp2Tls", true);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

var httpPort = builder.Configuration.GetValue("HttpPort", 5006);
var grpcPort = builder.Configuration.GetValue("GrpcPort", 5003);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(httpPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
    options.ListenAnyIP(grpcPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app);
app.Run();
