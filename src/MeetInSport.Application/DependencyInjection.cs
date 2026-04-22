using AutoMapper;
using MeetInSport.Application.Interface.Services;
using MeetInSport.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MeetInSport.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICoachService, CoachService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILessonPackageService, LessonPackageService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly()); // This line is added to register AutoMapper and scan the current assembly for mapping profiles.  
        return services;
    }
}

