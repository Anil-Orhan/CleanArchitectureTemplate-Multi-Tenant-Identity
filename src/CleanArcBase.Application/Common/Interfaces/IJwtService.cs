using CleanArcBase.Application.Common.Models;

namespace CleanArcBase.Application.Common.Interfaces;

public interface IJwtService
{
    TokenResult GenerateToken(
        Guid userId,
        Guid tenantId,
        string email,
        IEnumerable<string> roles,
        IEnumerable<string> permissions);

    string GenerateRefreshToken();
    bool ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
    Guid? GetTenantIdFromToken(string token);
}
