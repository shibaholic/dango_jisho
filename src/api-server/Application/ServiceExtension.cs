using System.Reflection;
using Application.Mappings.EntityDtos.Tracking;
using Application.Services;
using Application.UseCaseQueries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Application.Response;

namespace Application;

public static class ServiceExtension
{
    public static void ConfigureApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // Mediatr
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        
        // generic Mediatr handlers
        services.AddScoped(
            typeof(
                IRequestHandler<TagsByUserIdRequest<Tag_EITDto>, Response<List<Tag_EITDto>>>), 
            typeof(TagsByUserId<Tag_EITDto>));
        services.AddScoped(
            typeof(
                IRequestHandler<TagsByUserIdRequest<TagDto>, Response<List<TagDto>>>), 
            typeof(TagsByUserId<TagDto>));
        
        // CrudService
        services.AddTransient(typeof(ICrudService<,>), typeof(CrudService<,>));
        
        // other
        services.AddTransient(typeof(ITimeService), typeof(TimeService));
        services.AddScoped<IPasswordService, PasswordService>();
    }
}