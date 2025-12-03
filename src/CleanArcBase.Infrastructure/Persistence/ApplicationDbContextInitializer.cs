using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CleanArcBase.Domain.Entities.Identity;
using CleanArcBase.Domain.Entities.Tenancy;
using CleanArcBase.Domain.Enums;

namespace CleanArcBase.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly IConfiguration _configuration;

    public ApplicationDbContextInitializer(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<ApplicationDbContextInitializer> logger,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Skip migration for InMemory database (used in tests)
            var providerName = _context.Database.ProviderName;
            if (providerName != null && providerName.Contains("InMemory"))
            {
                await _context.Database.EnsureCreatedAsync();
                return;
            }

            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedPermissionsAsync();
            await SeedRolesAsync();
            await SeedDefaultRoleGroupsAsync();
            await SeedDefaultTenantAsync();
            await SeedDefaultUserAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedPermissionsAsync()
    {
        if (await _context.Permissions.AnyAsync())
            return;

        var permissions = new List<Permission>
        {
            // Users
            new("Users.Create", "Create Users", PermissionGroup.Users, "Allows creating new users"),
            new("Users.Read", "View Users", PermissionGroup.Users, "Allows viewing user list and details"),
            new("Users.Update", "Update Users", PermissionGroup.Users, "Allows updating user information"),
            new("Users.Delete", "Delete Users", PermissionGroup.Users, "Allows deleting users"),

            // Roles
            new("Roles.Create", "Create Roles", PermissionGroup.Roles, "Allows creating new roles"),
            new("Roles.Read", "View Roles", PermissionGroup.Roles, "Allows viewing role list and details"),
            new("Roles.Update", "Update Roles", PermissionGroup.Roles, "Allows updating role information"),
            new("Roles.Delete", "Delete Roles", PermissionGroup.Roles, "Allows deleting roles"),
            new("Roles.AssignPermissions", "Assign Permissions", PermissionGroup.Roles, "Allows assigning permissions to roles"),

            // Tenants
            new("Tenants.Create", "Create Tenants", PermissionGroup.Tenants, "Allows creating new tenants"),
            new("Tenants.Read", "View Tenant", PermissionGroup.Tenants, "Allows viewing tenant information"),
            new("Tenants.Update", "Update Tenant", PermissionGroup.Tenants, "Allows updating tenant settings"),

            // RoleGroups
            new("RoleGroups.Create", "Create Role Groups", PermissionGroup.Roles, "Allows creating new role groups"),
            new("RoleGroups.Read", "View Role Groups", PermissionGroup.Roles, "Allows viewing role group list and details"),
            new("RoleGroups.Update", "Update Role Groups", PermissionGroup.Roles, "Allows updating role group information"),
            new("RoleGroups.Delete", "Delete Role Groups", PermissionGroup.Roles, "Allows deleting role groups"),
            new("RoleGroups.AssignRoles", "Assign Roles to Groups", PermissionGroup.Roles, "Allows assigning roles to role groups"),

            // Reports
            new("Reports.View", "View Reports", PermissionGroup.Reports, "Allows viewing reports"),
            new("Reports.Export", "Export Reports", PermissionGroup.Reports, "Allows exporting reports")
        };

        await _context.Permissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} permissions", permissions.Count);
    }

    private async Task SeedRolesAsync()
    {
        var allPermissions = await _context.Permissions.ToListAsync();

        // SuperAdmin Role - All permissions, global
        if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
        {
            var superAdminRole = new ApplicationRole("SuperAdmin", "Super Administrator with all permissions", isSystemRole: true);
            await _roleManager.CreateAsync(superAdminRole);

            foreach (var permission in allPermissions)
            {
                superAdminRole.AssignPermission(permission.Id);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created SuperAdmin role with all permissions");
        }

        // TenantAdmin Role - All tenant permissions
        if (!await _roleManager.RoleExistsAsync("TenantAdmin"))
        {
            var tenantAdminRole = new ApplicationRole("TenantAdmin", "Tenant Administrator with all tenant permissions", isSystemRole: true);
            await _roleManager.CreateAsync(tenantAdminRole);

            var tenantPermissions = allPermissions.Where(p => p.Name != "Tenants.Update").ToList();
            foreach (var permission in tenantPermissions)
            {
                tenantAdminRole.AssignPermission(permission.Id);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created TenantAdmin role");
        }

        // Manager Role
        if (!await _roleManager.RoleExistsAsync("Manager"))
        {
            var managerRole = new ApplicationRole("Manager", "Manager with user and report permissions", isSystemRole: true);
            await _roleManager.CreateAsync(managerRole);

            var managerPermissions = allPermissions.Where(p =>
                p.Name.StartsWith("Users.") ||
                p.Name.StartsWith("Reports.")).ToList();

            foreach (var permission in managerPermissions)
            {
                managerRole.AssignPermission(permission.Id);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created Manager role");
        }

        // User Role - Read only
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            var userRole = new ApplicationRole("User", "Standard user with read permissions", isSystemRole: true);
            await _roleManager.CreateAsync(userRole);

            var readPermissions = allPermissions.Where(p => p.Name.EndsWith(".Read") || p.Name == "Reports.View").ToList();
            foreach (var permission in readPermissions)
            {
                userRole.AssignPermission(permission.Id);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created User role");
        }
    }

    private async Task SeedDefaultRoleGroupsAsync()
    {
        if (await _context.RoleGroups.AnyAsync())
            return;

        var superAdminRole = await _roleManager.FindByNameAsync("SuperAdmin");
        var tenantAdminRole = await _roleManager.FindByNameAsync("TenantAdmin");
        var managerRole = await _roleManager.FindByNameAsync("Manager");
        var userRole = await _roleManager.FindByNameAsync("User");

        // Administrators Group - SuperAdmin and TenantAdmin roles
        var adminsGroup = new RoleGroup(
            "Administrators",
            "Full system administrators with complete access",
            isSystem: true,
            displayOrder: 1);

        if (superAdminRole != null)
            adminsGroup.AssignRole(superAdminRole.Id);
        if (tenantAdminRole != null)
            adminsGroup.AssignRole(tenantAdminRole.Id);

        await _context.RoleGroups.AddAsync(adminsGroup);

        // Managers Group - Manager role
        var managersGroup = new RoleGroup(
            "Managers",
            "Managers with user and report access",
            isSystem: true,
            displayOrder: 2);

        if (managerRole != null)
            managersGroup.AssignRole(managerRole.Id);

        await _context.RoleGroups.AddAsync(managersGroup);

        // Standard Users Group - User role
        var usersGroup = new RoleGroup(
            "Standard Users",
            "Regular users with read-only access",
            isSystem: true,
            displayOrder: 3);

        if (userRole != null)
            usersGroup.AssignRole(userRole.Id);

        await _context.RoleGroups.AddAsync(usersGroup);

        await _context.SaveChangesAsync();
        _logger.LogInformation("Created default system role groups");
    }

    private async Task SeedDefaultTenantAsync()
    {
        if (await _context.Tenants.AnyAsync())
            return;

        var defaultTenant = new Tenant("Default Tenant", "default");
        await _context.Tenants.AddAsync(defaultTenant);
        await _context.SaveChangesAsync();

        // Clone system role groups to default tenant
        var systemRoleGroups = await _context.RoleGroups
            .Include(rg => rg.RoleGroupRoles)
            .Where(rg => rg.IsSystem && rg.TenantId == null)
            .ToListAsync();

        foreach (var systemGroup in systemRoleGroups)
        {
            var clonedGroup = systemGroup.Clone(defaultTenant.Id);
            await _context.RoleGroups.AddAsync(clonedGroup);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Created default tenant with {Count} role groups", systemRoleGroups.Count);
    }

    private async Task SeedDefaultUserAsync()
    {
        var defaultTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Identifier == "default");
        if (defaultTenant == null)
            return;

        var adminEmail = _configuration["DefaultAdmin:Email"] ?? "admin@CleanArcBase.com";
        var adminPassword = _configuration["DefaultAdmin:Password"]
            ?? throw new InvalidOperationException("DefaultAdmin:Password is not configured. Please set it in appsettings.json or environment variables.");
        var adminFirstName = _configuration["DefaultAdmin:FirstName"] ?? "System";
        var adminLastName = _configuration["DefaultAdmin:LastName"] ?? "Administrator";

        var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin == null)
        {
            var adminUser = new ApplicationUser(
                defaultTenant.Id,
                adminEmail,
                adminFirstName,
                adminLastName);

            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                _logger.LogInformation("Created default admin user: {Email}", adminEmail);
            }
            else
            {
                _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
