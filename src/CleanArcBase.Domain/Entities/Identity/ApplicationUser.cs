using CleanArcBase.Domain.Entities.Tenancy;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace CleanArcBase.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; private set; }
    public Tenant Tenant { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";

    private ApplicationUser() { }

    public ApplicationUser(
        Guid tenantId,
        string email,
        string firstName,
        string lastName,
        string? userName = null)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        Id = NewId.NextGuid();
        TenantId = tenantId;
        Email = email;
        UserName = userName ?? email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUpdatedBy(string userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCreatedBy(string userId)
    {
        CreatedBy = userId;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void AddRefreshToken(RefreshToken token)
    {
        _refreshTokens.Add(token);
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
        {
            token.Revoke();
        }
    }
}
