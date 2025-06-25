using Moq;
using MongoDB.Driver;
using EFTest.Domain.Entities;
using EFTest.Domain.ValueObjects;
using EFTest.Infrastructure.Repositories;
using EFTest.Infrastructure.Services;

namespace EFTest.Tests;

public class OrderRepositoryTests
{
    private readonly Mock<IMongoDbService> _mockMongoDbService;
    private readonly Mock<IMongoCollection<Order>> _mockCollection;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        _mockMongoDbService = new Mock<IMongoDbService>();
        _mockCollection = new Mock<IMongoCollection<Order>>();

        _mockMongoDbService.Setup(x => x.Orders).Returns(_mockCollection.Object);
        _repository = new OrderRepository(_mockMongoDbService.Object);
    }

    [Fact]
    public void Constructor_WithNullMongoDbService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OrderRepository(null!));
    }

    [Fact]
    public async Task AddAsync_WithNullOrder_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_WithNullOrder_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_WithNullOrder_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync(null!));
    }

    [Fact]
    public void Repository_CreatesSuccessfully_WithValidMongoDbService()
    {
        // Arrange & Act
        var repository = new OrderRepository(_mockMongoDbService.Object);

        // Assert
        Assert.NotNull(repository);
    }
}
