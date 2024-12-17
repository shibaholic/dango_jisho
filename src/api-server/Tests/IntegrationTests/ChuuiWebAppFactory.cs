using Infrastructure.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests;

public class ChuuiWebAppFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context,services) =>
        {
            var configuration = context.Configuration;
            var connectionString = configuration.GetConnectionString("PostgreSQL");
            
            Assert.NotNull(connectionString);
            Assert.NotEmpty(connectionString);
            
            // Remove the app's DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MyDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            // Add a test database
            services.AddDbContext<MyDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create the database and seed test data
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<MyDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}