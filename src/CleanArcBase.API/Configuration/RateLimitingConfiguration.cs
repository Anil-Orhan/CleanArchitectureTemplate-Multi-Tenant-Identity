using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace CleanArcBase.API.Configuration;

public static class RateLimitingConfiguration
{
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Public endpoints (login, refresh) - IP based - 20/dk
            options.AddFixedWindowLimiter("Public", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 20;
                opt.QueueLimit = 0;
            });

            // Standard authenticated endpoints - User based - 120/dk
            options.AddFixedWindowLimiter("Standard", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 120;
                opt.QueueLimit = 2;
            });

            // Write operations - User based - 40/dk
            options.AddFixedWindowLimiter("Write", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 40;
                opt.QueueLimit = 0;
            });

            // Sensitive operations (role/permission changes) - 10/dk
            options.AddFixedWindowLimiter("Sensitive", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);
                opt.PermitLimit = 10;
                opt.QueueLimit = 0;
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                var retryAfterSeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                    ? (int)retryAfter.TotalSeconds
                    : 60;

                context.HttpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString();

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests. Please try again later.",
                    retryAfter = retryAfterSeconds
                }, token);
            };
        });

        return services;
    }
}
