using FluentAssertions;
using CleanArcBase.Domain.Entities.Identity;

namespace CleanArcBase.Domain.Tests.Entities;

public class ApplicationRoleTests
{
    [Fact]
    public void Constructor_ShouldCreateRole_WithValidParameters()
    {
        // Arrange
        var name = "TestRole";
        var description = "Test Description";
        var isSystemRole = false;
        var tenantId = Guid.NewGuid();

        // Act
        var role = new ApplicationRole(name, description, isSystemRole, tenantId);

        // Assert
        role.Id.Should().NotBeEmpty();
        role.Name.Should().Be(name);
        role.NormalizedName.Should().Be(name.ToUpperInvariant());
        role.Description.Should().Be(description);
        role.IsSystemRole.Should().BeFalse();
        role.TenantId.Should().Be(tenantId);
        role.RolePermissions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldCreateSystemRole_WhenIsSystemRoleTrue()
    {
        // Arrange & Act
        var role = new ApplicationRole("SuperAdmin", "Admin role", isSystemRole: true);

        // Assert
        role.IsSystemRole.Should().BeTrue();
        role.TenantId.Should().BeNull();
    }

    [Fact]
    public void Update_ShouldUpdateNameAndDescription()
    {
        // Arrange
        var role = new ApplicationRole("OldName", "Old Description");
        var newName = "NewName";
        var newDescription = "New Description";

        // Act
        role.Update(newName, newDescription);

        // Assert
        role.Name.Should().Be(newName);
        role.NormalizedName.Should().Be(newName.ToUpperInvariant());
        role.Description.Should().Be(newDescription);
    }

    [Fact]
    public void AssignPermission_ShouldAddPermission_WhenNotExists()
    {
        // Arrange
        var role = new ApplicationRole("TestRole");
        var permissionId = Guid.NewGuid();

        // Act
        role.AssignPermission(permissionId);

        // Assert
        role.RolePermissions.Should().ContainSingle();
        role.RolePermissions.First().PermissionId.Should().Be(permissionId);
    }

    [Fact]
    public void AssignPermission_ShouldNotDuplicate_WhenPermissionAlreadyExists()
    {
        // Arrange
        var role = new ApplicationRole("TestRole");
        var permissionId = Guid.NewGuid();

        // Act
        role.AssignPermission(permissionId);
        role.AssignPermission(permissionId); // Assign same permission again

        // Assert
        role.RolePermissions.Should().ContainSingle();
    }

    [Fact]
    public void RemovePermission_ShouldRemovePermission_WhenExists()
    {
        // Arrange
        var role = new ApplicationRole("TestRole");
        var permissionId = Guid.NewGuid();
        role.AssignPermission(permissionId);

        // Act
        role.RemovePermission(permissionId);

        // Assert
        role.RolePermissions.Should().BeEmpty();
    }

    [Fact]
    public void RemovePermission_ShouldDoNothing_WhenPermissionNotExists()
    {
        // Arrange
        var role = new ApplicationRole("TestRole");
        var existingPermissionId = Guid.NewGuid();
        var nonExistingPermissionId = Guid.NewGuid();
        role.AssignPermission(existingPermissionId);

        // Act
        role.RemovePermission(nonExistingPermissionId);

        // Assert
        role.RolePermissions.Should().ContainSingle();
        role.RolePermissions.First().PermissionId.Should().Be(existingPermissionId);
    }

    [Fact]
    public void ClearPermissions_ShouldRemoveAllPermissions()
    {
        // Arrange
        var role = new ApplicationRole("TestRole");
        role.AssignPermission(Guid.NewGuid());
        role.AssignPermission(Guid.NewGuid());
        role.AssignPermission(Guid.NewGuid());

        // Act
        role.ClearPermissions();

        // Assert
        role.RolePermissions.Should().BeEmpty();
    }
}
