using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Interfaces.Repositories;
using CleanArcBase.Domain.Entities.Identity;
using CleanArcBase.Infrastructure.Identity;
using CleanArcBase.Infrastructure.Persistence;
using CleanArcBase.Infrastructure.Persistence.Repositories;
using CleanArcBase.Infrastructure.Services;

namespace CleanArcBase.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        string? environmentName = null)
    {
        // Skip database registration for Testing environment (handled by test project)
        if (environmentName != "Testing")
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Repositories
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleGroupRepository, RoleGroupRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Identity
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // JWT Settings
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JWT settings not configured");

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Authentication
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        // Authorization
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // Services
        services.AddTransient<IDateTimeService, DateTimeService>();
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();

        // Database Initializer
        services.AddScoped<ApplicationDbContextInitializer>();

        return services;
    }
}
