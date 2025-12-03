# Clean Architecture Template - Multi-Tenant Identity

A production-ready .NET 9.0 template for building multi-tenant SaaS applications with comprehensive identity management, role-based access control (RBAC), and permission-based authorization.

## Features

### Authentication & Authorization
- JWT-based authentication with access and refresh tokens
- Refresh token rotation with expiration tracking
- Token revocation capability
- User activation/deactivation
- Configurable password policies

### Multi-Tenancy
- Database-level tenant isolation using EF Core global query filters
- Automatic tenant context extraction from JWT claims
- Tenant-specific data filtering
- System-level and tenant-specific resources support

### Role-Based Access Control (RBAC)
- Complete role management (CRUD operations)
- Permission assignment to roles
- User-to-role assignment
- Multi-tenant role support

### Permission-Based Authorization
- Fine-grained permission system
- Permission grouping (Users, Roles, Tenants, Reports)
- Dynamic authorization policies
- `[Permission("Users.Read")]` attribute decorator for controller actions

### Role Groups
- Organizational grouping of multiple roles
- System-level role groups (shared across tenants)
- Tenant-specific role group cloning
- Bulk role assignment via groups

### Rate Limiting
- Four-tier rate limiting configuration:
  - **Public**: 20 requests/minute (login, refresh)
  - **Standard**: 120 requests/minute (authenticated)
  - **Write**: 40 requests/minute (data modifications)
  - **Sensitive**: 10 requests/minute (role/permission changes)

## Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:

```
src/
├── CleanArcBase.Domain/           # Business logic & entities
│   ├── Entities/
│   │   ├── Identity/              # User, Role, Permission, RefreshToken, RoleGroup
│   │   └── Tenancy/               # Tenant entity
│   ├── Common/                    # Base entities, domain events
│   └── Enums/
│
├── CleanArcBase.Application/      # Use cases & business rules
│   ├── Common/
│   │   ├── Interfaces/            # Abstractions (IApplicationDbContext, IIdentityService, etc.)
│   │   ├── Behaviors/             # MediatR pipeline behaviors
│   │   ├── Models/                # DTOs
│   │   └── Mappings/              # AutoMapper profiles
│   └── Features/                  # CQRS Commands & Queries
│       ├── Permissions/
│       ├── Roles/
│       ├── RoleGroups/
│       └── Tenants/
│
├── CleanArcBase.Infrastructure/   # Technical implementation
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/        # EF Fluent API configurations
│   │   ├── Repositories/          # Generic & specialized repositories
│   │   └── Migrations/
│   ├── Identity/                  # JWT, permissions, authorization handlers
│   └── Services/
│
└── CleanArcBase.API/              # Presentation layer
    ├── Controllers/               # REST API endpoints
    ├── Middleware/                # Exception handling, tenant extraction
    └── Filters/                   # Permission attribute

tests/
├── CleanArcBase.Domain.Tests/
├── CleanArcBase.Application.Tests/
└── CleanArcBase.API.Tests/
```

## Technology Stack

| Category | Technology |
|----------|------------|
| Framework | .NET 9.0, ASP.NET Core 9.0 |
| Database | PostgreSQL with Entity Framework Core 9.0 |
| Authentication | JWT Bearer Tokens, ASP.NET Core Identity |
| CQRS | MediatR 13.1 |
| Validation | FluentValidation 12.1 |
| Mapping | AutoMapper 15.1 |
| API Documentation | Swagger/Swashbuckle |
| Testing | xUnit, FluentAssertions |

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 13+](https://www.postgresql.org/download/)

### Configuration

Update `appsettings.json` in the API project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=your_db;Username=your_user;Password=your_password;"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
    "Issuer": "CleanArcBase.API",
    "Audience": "CleanArcBase.Client",
    "AccessTokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  },
  "DefaultAdmin": {
    "Email": "admin@example.com",
    "Password": "YourSecurePassword123!",
    "FirstName": "System",
    "LastName": "Administrator"
  }
}
```

### Running the Application

```bash
# Clone the repository
git clone https://github.com/your-username/CleanArchitectureTemplate-Multi-Tenant-Identity.git
cd CleanArchitectureTemplate-Multi-Tenant-Identity

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update --project src/CleanArcBase.Infrastructure --startup-project src/CleanArcBase.API

# Run the application
dotnet run --project src/CleanArcBase.API
```

The API will be available at `https://localhost:5001` with Swagger UI at `/swagger`.

### Running Tests

```bash
dotnet test
```

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Logout (revoke refresh token) |

### Users
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create new user |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |
| POST | `/api/users/{id}/roles` | Assign roles to user |

### Roles
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/roles` | Get all roles |
| GET | `/api/roles/{id}` | Get role by ID |
| POST | `/api/roles` | Create new role |
| PUT | `/api/roles/{id}` | Update role |
| DELETE | `/api/roles/{id}` | Delete role |
| POST | `/api/roles/{id}/permissions` | Assign permissions to role |

### Permissions
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/permissions` | Get all permissions |
| GET | `/api/permissions/groups` | Get permissions grouped |

### Tenants
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tenants` | Get all tenants |
| GET | `/api/tenants/{id}` | Get tenant by ID |
| POST | `/api/tenants` | Create new tenant |
| PUT | `/api/tenants/{id}` | Update tenant |
| DELETE | `/api/tenants/{id}` | Delete tenant |

### Role Groups
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/rolegroups` | Get all role groups |
| POST | `/api/rolegroups` | Create new role group |
| POST | `/api/rolegroups/{id}/roles` | Assign roles to group |

## Multi-Tenancy Implementation

### How It Works

1. **Tenant Context Middleware**: Extracts tenant claim from JWT token after authentication
2. **Global Query Filters**: EF Core automatically filters all queries by current tenant
3. **Tenant Isolation**: Each tenant's data is completely isolated at the database query level

```csharp
// Automatic filtering in ApplicationDbContext
modelBuilder.Entity<ApplicationUser>()
    .HasQueryFilter(u => u.TenantId == _currentTenantService.TenantId);
```

### System vs Tenant Resources

- **System-level** (TenantId = null): Visible to all tenants (e.g., system role groups)
- **Tenant-specific**: Only visible to the owning tenant

## Permission System

### Using Permission Attribute

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    [HttpGet]
    [Permission("Users.Read")]
    public async Task<IActionResult> GetAll()
    {
        // Only users with Users.Read permission can access
    }

    [HttpPost]
    [Permission("Users.Create")]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        // Only users with Users.Create permission can access
    }
}
```

### Available Permission Groups

- `Users` - User management permissions
- `Roles` - Role management permissions
- `Tenants` - Tenant management permissions
- `Reports` - Reporting permissions

## Design Patterns

- **Clean Architecture**: Clear layer separation with dependency inversion
- **CQRS**: Command Query Responsibility Segregation via MediatR
- **Repository Pattern**: Generic repository with Unit of Work
- **Domain Events**: Support for domain event handling
- **Auditable Entities**: Automatic tracking of CreatedAt, UpdatedAt, CreatedBy, UpdatedBy

## Password Policy

Default password requirements:
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Robert C. Martin
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
