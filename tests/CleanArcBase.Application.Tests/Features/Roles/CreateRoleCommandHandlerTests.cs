using FluentAssertions;
using Moq;
using CleanArcBase.Application.Common.Interfaces;
using CleanArcBase.Application.Common.Interfaces.Repositories;
using CleanArcBase.Application.Features.Roles.Commands.CreateRole;
using CleanArcBase.Domain.Entities.Identity;

namespace CleanArcBase.Application.Tests.Features.Roles;

public class CreateRoleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly CreateRoleCommandHandler _handler;

    public CreateRoleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _roleRepositoryMock = new Mock<IRoleRepository>();

        _unitOfWorkMock.Setup(u => u.Roles).Returns(_roleRepositoryMock.Object);

        _handler = new CreateRoleCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateRole_WhenNameIsUnique()
    {
        // Arrange
        var command = new CreateRoleCommand("NewRole", "Description", null);

        _roleRepositoryMock
            .Setup(r => r.IsNameUniqueAsync(command.Name, command.TenantId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationRole role, CancellationToken _) => role);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(command.Name);
        result.Value.Description.Should().Be(command.Description);
        result.Value.IsSystemRole.Should().BeFalse();

        _roleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameIsNotUnique()
    {
        // Arrange
        var command = new CreateRoleCommand("ExistingRole", "Description", null);

        _roleRepositoryMock
            .Setup(r => r.IsNameUniqueAsync(command.Name, command.TenantId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already exists");

        _roleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateTenantSpecificRole_WhenTenantIdProvided()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var command = new CreateRoleCommand("TenantRole", "Tenant specific role", tenantId);

        _roleRepositoryMock
            .Setup(r => r.IsNameUniqueAsync(command.Name, command.TenantId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<ApplicationRole>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationRole role, CancellationToken _) => role);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TenantId.Should().Be(tenantId);
    }
}
