using Microsoft.AspNetCore.Authorization;

namespace CleanArcBase.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermissionAttribute : AuthorizeAttribute
{
    private const string PolicyPrefix = "Permission:";

    public PermissionAttribute(string permission)
        : base(PolicyPrefix + permission)
    {
    }
}
