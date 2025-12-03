using CleanArcBase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CleanArcBase.API.Tests;


public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove ApplicationDbContext and its options
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();

            // Remove all EF Core related services that contain database provider registrations
            var efServiceTypes = services
                .Where(d =>
                    d.ServiceType.FullName?.StartsWith("Microsoft.EntityFrameworkCore") == true ||
                    d.ServiceType.FullName?.Contains("Npgsql") == true ||
                    d.ImplementationType?.FullName?.Contains("Npgsql") == true)
                .ToList();

            foreach (var descriptor in efServiceTypes)
            {
                services.Remove(descriptor);
            }

            // Add fresh in-memory database for testing
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
            });
        });

        builder.UseEnvironment("Testing");
    }
}
