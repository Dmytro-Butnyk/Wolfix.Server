using Admin.Application.Dto.Requests;
using Admin.Application.Dto.Responses;
using Admin.Application.Services;
using Admin.Domain.Interfaces;
using Admin.Domain.Projections;
using Admin.IntegrationEvents;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Application.Dto;
using Shared.Domain.Models;
using Shared.Domain.ValueObjects;
using Shared.IntegrationEvents;
using Shared.IntegrationEvents.Interfaces;
using Xunit;
using AdminAggregate = Admin.Domain.AdminAggregate.Admin;

namespace Admin.Tests.Application;

public class AdminServiceTests
{
    private readonly Mock<IAdminRepository> _adminRepositoryMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
    private readonly EventBus _eventBus;
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        _adminRepositoryMock = new Mock<IAdminRepository>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

        _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);

        _eventBus = new EventBus(_serviceScopeFactoryMock.Object);
        
        _adminService = new AdminService(_adminRepositoryMock.Object, _eventBus);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenAllOperationsSucceed()
    {
        // Arrange
        CreateAdminDto request = new("test@example.com", "HUgha733821", "John", "Doe", "Middle", "+380970878346");
        Guid accountId = Guid.NewGuid();
        CancellationToken ct = CancellationToken.None;

        Mock<IIntegrationEventHandler<CreateAdmin, Guid>> createAdminHandlerMock = new();
        createAdminHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<CreateAdmin>(), ct))
            .ReturnsAsync(Result<Guid>.Success(accountId));

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IIntegrationEventHandler<CreateAdmin, Guid>)))
            .Returns(createAdminHandlerMock.Object);
        
        _adminRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<AdminAggregate>(), ct))
            .Returns(Task.CompletedTask);
        _adminRepositoryMock
            .Setup(x => x.SaveChangesAsync(ct))
            .Returns(Task.CompletedTask);

        // Act
        VoidResult result = await _adminService.CreateAsync(request, ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _adminRepositoryMock.Verify(x => x.AddAsync(It.Is<AdminAggregate>(a => 
            a.AccountId == accountId && 
            a.FullName.FirstName == request.FirstName &&
            a.PhoneNumber.Value == request.PhoneNumber), ct), Times.Once);
        _adminRepositoryMock.Verify(x => x.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenCreateAccountFails()
    {
        // Arrange
        CreateAdminDto request = new("test@example.com", "Password123!", "John", "Doe", "Middle", "1234567890");
        CancellationToken ct = CancellationToken.None;
        string errorMessage = "Account creation failed";

        Mock<IIntegrationEventHandler<CreateAdmin, Guid>> createAdminHandlerMock = new();
        createAdminHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<CreateAdmin>(), ct))
            .ReturnsAsync(Result<Guid>.Failure(errorMessage));

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IIntegrationEventHandler<CreateAdmin, Guid>)))
            .Returns(createAdminHandlerMock.Object);

        // Act
        VoidResult result = await _adminService.CreateAsync(request, ct);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be(errorMessage);
        _adminRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AdminAggregate>(), ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenAdminAggregateCreationFails()
    {
        // Arrange
        CreateAdminDto request = new("test@example.com", "Password123!", "John", "Doe", "Middle", ""); 
        Guid accountId = Guid.NewGuid();
        CancellationToken ct = CancellationToken.None;

        Mock<IIntegrationEventHandler<CreateAdmin, Guid>> createAdminHandlerMock = new();
        createAdminHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<CreateAdmin>(), ct))
            .ReturnsAsync(Result<Guid>.Success(accountId));

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IIntegrationEventHandler<CreateAdmin, Guid>)))
            .Returns(createAdminHandlerMock.Object);

        // Act
        VoidResult result = await _adminService.CreateAsync(request, ct);

        // Assert
        result.IsFailure.Should().BeTrue();
        _adminRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AdminAggregate>(), ct), Times.Never);
    }

    [Fact]
    public async Task GetForPageAsync_ShouldReturnEmptyPagination_WhenTotalCountIsZero()
    {
        // Arrange
        int page = 1;
        int pageSize = 10;
        CancellationToken ct = CancellationToken.None;

        _adminRepositoryMock
            .Setup(x => x.GetBasicAdminsTotalCountAsync(ct))
            .ReturnsAsync(0);

        // Act
        PaginationDto<BasicAdminDto> result = await _adminService.GetForPageAsync(page, pageSize, ct);

        // Assert
        result.TotalItems.Should().Be(0);
        result.Items.Should().BeEmpty();
        _adminRepositoryMock.Verify(x => x.GetForPageAsync(It.IsAny<int>(), It.IsAny<int>(), ct), Times.Never);
    }

    [Fact]
    public async Task GetForPageAsync_ShouldReturnPaginationWithItems_WhenItemsExist()
    {
        // Arrange
        int page = 1;
        int pageSize = 10;
        CancellationToken ct = CancellationToken.None;
        int totalCount = 1;
        
        FullName fullName = FullName.Create("John", "Doe", "Middle").Value!;
        PhoneNumber phoneNumber = PhoneNumber.Create("1234567890").Value!;
        BasicAdminProjection projection = new(Guid.NewGuid(), fullName, phoneNumber);
        List<BasicAdminProjection> projections = [projection];

        _adminRepositoryMock
            .Setup(x => x.GetBasicAdminsTotalCountAsync(ct))
            .ReturnsAsync(totalCount);

        _adminRepositoryMock
            .Setup(x => x.GetForPageAsync(page, pageSize, ct))
            .ReturnsAsync(projections);

        // Act
        PaginationDto<BasicAdminDto> result = await _adminService.GetForPageAsync(page, pageSize, ct);

        // Assert
        result.TotalItems.Should().Be(totalCount);
        result.Items.Should().HaveCount(1);
        result.Items.First().FirstName.Should().Be("John");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenAdminNotFound()
    {
        // Arrange
        Guid adminId = Guid.NewGuid();
        CancellationToken ct = CancellationToken.None;

        _adminRepositoryMock
            .Setup(x => x.GetByIdAsync(adminId, ct, It.IsAny<string[]>()))
            .ReturnsAsync((AdminAggregate?)null);

        // Act
        VoidResult result = await _adminService.DeleteAsync(adminId, ct);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenAdminDeletedSuccessfully()
    {
        // Arrange
        Guid adminId = Guid.NewGuid();
        Guid accountId = Guid.NewGuid();
        CancellationToken ct = CancellationToken.None;

        Result<AdminAggregate> adminResult = AdminAggregate.Create(accountId, "John", "Doe", "Middle", "1234567890");
        AdminAggregate admin = adminResult.Value!;

        _adminRepositoryMock
            .Setup(x => x.GetByIdAsync(adminId, ct, It.IsAny<string[]>()))
            .ReturnsAsync(admin);

        Mock<IIntegrationEventHandler<DeleteAdminAccount>> deleteAdminHandlerMock = new();
        deleteAdminHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<DeleteAdminAccount>(), ct))
            .ReturnsAsync(VoidResult.Success());

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IEnumerable<IIntegrationEventHandler<DeleteAdminAccount>>)))
            .Returns(new List<IIntegrationEventHandler<DeleteAdminAccount>> { deleteAdminHandlerMock.Object });

        // Act
        VoidResult result = await _adminService.DeleteAsync(adminId, ct);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _adminRepositoryMock.Verify(x => x.Delete(admin, ct), Times.Once);
        _adminRepositoryMock.Verify(x => x.SaveChangesAsync(ct), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenDeleteAccountEventFails()
    {
        // Arrange
        Guid adminId = Guid.NewGuid();
        Guid accountId = Guid.NewGuid();
        CancellationToken ct = CancellationToken.None;
        string errorMessage = "Delete failed";

        Result<AdminAggregate> adminResult = AdminAggregate.Create(accountId, "John", "Doe", "Middle", "1234567890");
        AdminAggregate admin = adminResult.Value!;

        _adminRepositoryMock
            .Setup(x => x.GetByIdAsync(adminId, ct, It.IsAny<string[]>()))
            .ReturnsAsync(admin);

        Mock<IIntegrationEventHandler<DeleteAdminAccount>> deleteAdminHandlerMock = new();
        deleteAdminHandlerMock
            .Setup(x => x.HandleAsync(It.IsAny<DeleteAdminAccount>(), ct))
            .ReturnsAsync(VoidResult.Failure(errorMessage));

        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IEnumerable<IIntegrationEventHandler<DeleteAdminAccount>>)))
            .Returns(new List<IIntegrationEventHandler<DeleteAdminAccount>> { deleteAdminHandlerMock.Object });

        // Act
        VoidResult result = await _adminService.DeleteAsync(adminId, ct);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be(errorMessage);
        _adminRepositoryMock.Verify(x => x.Delete(It.IsAny<AdminAggregate>(), ct), Times.Never);
    }
}