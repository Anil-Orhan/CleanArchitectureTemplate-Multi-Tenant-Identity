using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Interfaces.Repositories;

namespace CleanArcBase.API.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenantService tenantService,
        ITenantRepository tenantRepository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant");
            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantId))
            {
                var tenant = await tenantRepository.GetByIdAsync(tenantId);
                if (tenant != null && tenant.IsActive)
                {
                    tenantService.SetTenant(tenant.Id, tenant.Identifier);
                }
            }
        }

        await _next(context);
    }
}

public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}
