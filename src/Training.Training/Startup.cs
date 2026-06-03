using Training.Training.Grpc;
using Training.Training.Interceptors;

namespace Training.Training;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });
        services.AddGrpcReflection();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
    }

    public void Configure(WebApplication app)
    {
        app.MapGrpcService<TrainingGrpcService>();
        app.MapGrpcReflectionService();
        app.MapGet("/health", () => "OK");
    }
}
