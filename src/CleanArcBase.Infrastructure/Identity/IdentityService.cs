using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Models;
using CleanArcBase.Domain.Entities.Identity;
using CleanArcBase.Infrastructure.Persistence;

namespace CleanArcBase.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPermissionService _permissionService;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context,
        IJwtService jwtService,
        IPermissionService permissionService,
        Microsoft.Extensions.Options.IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtService = jwtService;
        _permissionService = permissionService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user == null)
            return AuthResult.Failure("Invalid email or password");

        if (!user.IsActive)
            return AuthResult.Failure("User account is deactivated");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
            return AuthResult.Failure("Invalid email or password");

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        var tokenResult = _jwtService.GenerateToken(
            user.Id,
            user.TenantId,
            user.Email!,
            roles,
            permissions);

        var refreshToken = new RefreshToken(
            user.Id,
            tokenResult.RefreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays));

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return AuthResult.Success(tokenResult.AccessToken, tokenResult.RefreshToken, tokenResult.ExpiresAt);
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (token == null)
            return AuthResult.Failure("Invalid refresh token");

        if (!token.IsActive)
            return AuthResult.Failure("Refresh token is expired or revoked");

        var user = token.User;
        if (!user.IsActive)
            return AuthResult.Failure("User account is deactivated");

        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        var tokenResult = _jwtService.GenerateToken(
            user.Id,
            user.TenantId,
            user.Email!,
            roles,
            permissions);

        token.Revoke(tokenResult.RefreshToken);

        var newRefreshToken = new RefreshToken(
            user.Id,
            tokenResult.RefreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays));

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return AuthResult.Success(tokenResult.AccessToken, tokenResult.RefreshToken, tokenResult.ExpiresAt);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (token == null || !token.IsActive)
            return false;

        token.Revoke();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(bool Succeeded, string? UserId, IEnumerable<string> Errors)> CreateUserAsync(
        Guid tenantId,
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser(tenantId, email, firstName, lastName);

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, null, result.Errors.Select(e => e.Description));

        return (true, user.Id.ToString(), Array.Empty<string>());
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            return null;

        return await MapToUserDtoAsync(user, cancellationToken);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user == null)
            return null;

        return await MapToUserDtoAsync(user, cancellationToken);
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .IgnoreQueryFilters()
            .Where(u => u.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        if (!users.Any())
            return Enumerable.Empty<UserDto>();

        var userIds = users.Select(u => u.Id).ToList();

        // Batch load all roles for all users in single query
        var userRoles = await _context.UserRoles
            .Where(ur => userIds.Contains(ur.UserId))
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, RoleName = r.Name })
            .ToListAsync(cancellationToken);

        var userRolesDict = userRoles
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.RoleName!).ToList());

        // Batch load all permissions for all users in single query
        var userPermissions = await (
            from ur in _context.UserRoles
            join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where userIds.Contains(ur.UserId)
            select new { ur.UserId, PermissionName = p.Name }
        ).ToListAsync(cancellationToken);

        var userPermissionsDict = userPermissions
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.PermissionName).Distinct().ToList());

        return users.Select(user => new UserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            IsActive = user.IsActive,
            Roles = userRolesDict.GetValueOrDefault(user.Id, new List<string>()),
            Permissions = userPermissionsDict.GetValueOrDefault(user.Id, new List<string>())
        });
    }

    public async Task<bool> AssignRolesToUserAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var result = await _userManager.AddToRolesAsync(user, roles);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRolesFromUserAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return false;

        var result = await _userManager.RemoveFromRolesAsync(user, roles);
        return result.Succeeded;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Enumerable.Empty<string>();

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users.IgnoreQueryFilters().Where(u => u.Email == email);

        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    private async Task<UserDto> MapToUserDtoAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _permissionService.GetUserPermissionsAsync(user.Id, cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            IsActive = user.IsActive,
            Roles = roles,
            Permissions = permissions
        };
    }
}
