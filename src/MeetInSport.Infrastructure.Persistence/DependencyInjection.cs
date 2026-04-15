using MeetInSport.Application.Interface.Repositories;
using MeetInSport.Infrastructure.Persistence.Repositories;
using MeetInSport.Infrastructure.Persistence.Seeders;
using Microsoft.Extensions.DependencyInjection;


namespace MeetInSport.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register the generic repository.
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Register specific repositories.
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICoachRepository, CoachRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddTransient<DataBaseSeeder>();  // AddTrasient provides us to throw these information away from the memoryç

        return services;
    }
}