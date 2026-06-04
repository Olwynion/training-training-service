using Training.Training.Domain.Repositories;
using Training.Training.Domain.Services;
using Training.Training.Infrastructure.Factories;
using Training.Training.Infrastructure.Repositories;
using Training.Training.Infrastructure.Services;

namespace Training.Training;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IWorkoutPlanRepository, WorkoutPlanRepository>();
        services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();
        services.AddScoped<IOneRmRepository, OneRmRepository>();
        services.AddScoped<ICycleCalculator, CycleCalculator>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
