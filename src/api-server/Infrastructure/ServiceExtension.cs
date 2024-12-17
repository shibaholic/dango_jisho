using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Repositories;
using Infrastructure.DbContext;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
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
                options.UseNpgsql(connectionString //, 
                    // x => x.MigrationsAssembly("Project.Infrastructure")
                );
            }
        );

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEntryRepository, EntryRepository>();
    }
}