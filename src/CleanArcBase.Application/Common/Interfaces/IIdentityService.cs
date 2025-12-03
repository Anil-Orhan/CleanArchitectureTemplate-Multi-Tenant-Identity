using CleanArcBase.Application.Common.Models;

namespace CleanArcBase.Application.Common.Interfaces;

public interface IIdentityService
{
    // Authentication
    Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    // User Management
    Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> CreateUserAsync(
        Guid tenantId,
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetUsersAsync(Guid tenantId, CancellationToken cancellationToken = default);

    // Role Management
    Task<bool> AssignRolesToUserAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<bool> RemoveRolesFromUserAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    // Validation
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
}
