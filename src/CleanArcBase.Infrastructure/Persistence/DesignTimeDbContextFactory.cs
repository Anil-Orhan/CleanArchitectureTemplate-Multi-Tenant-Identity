using CleanArcBase.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CleanArcBase.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CleanArcBase.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(
            optionsBuilder.Options,
            new DesignTimeCurrentUserService(),
            new DesignTimeCurrentTenantService(),
            new DesignTimeDateTimeService());
    }
}

internal class DesignTimeCurrentUserService : ICurrentUserService
{
    public string? UserId => "design-time";
    public string? UserName => "Design Time User";
}

internal class DesignTimeCurrentTenantService : ICurrentTenantService
{
    public Guid? TenantId => null;
    public string? TenantIdentifier => null;
    public void SetTenant(Guid tenantId, string identifier) { }
}

internal class DesignTimeDateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
