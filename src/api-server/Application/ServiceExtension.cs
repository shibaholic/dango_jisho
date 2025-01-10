using System.Reflection;
using Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceExtension
{
    public static void ConfigureApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // Mediatr
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // CrudService
        services.AddTransient(typeof(ICrudService<,>), typeof(CrudService<,>));
        
        // other
        services.AddTransient(typeof(ITimeService), typeof(TimeService));
    }
}