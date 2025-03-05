using Domain.RepositoryInterfaces;
using Infrastructure.Repositories;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceExtensions
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        IServiceCollection serviceCollection = services.AddDbContext<MyDbContext>(options =>
            {
                options.UseNpgsql(connectionString, 
                    x => x.MigrationsAssembly("Infrastructure")
                );
            }
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEntryRepository, EntryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ITrackingRepository, TrackingRepository>();
        services.AddScoped<IStudySetRepository, StudySetRepository>();
    }
}