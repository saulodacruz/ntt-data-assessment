using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

/// <summary>
/// Integration tests for SaleRepository using in-memory database.
/// These tests verify the repository works correctly with Entity Framework Core.
/// </summary>
public class SaleRepositoryIntegrationTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly ISaleRepository _repository;

    public SaleRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
    }

    [Fact(DisplayName = "Given valid sale When creating Then sale should be persisted in database")]
    public async Task CreateAsync_ValidSale_ShouldPersistInDatabase()
    {
        // Arrange
        var sale = CreateTestSale();

        // Act
        var created = await _repository.CreateAsync(sale);

        // Assert
        created.Should().NotBeNull();
        created.Id.Should().NotBeEmpty();

        var retrieved = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == created.Id);

        retrieved.Should().NotBeNull();
        retrieved!.SaleNumber.Should().Be(sale.SaleNumber);
        retrieved.Items.Should().HaveCount(sale.Items.Count);
    }

    [Fact(DisplayName = "Given existing sale id When getting by id Then should return sale with items")]
    public async Task GetByIdAsync_ExistingSaleId_ShouldReturnSaleWithItems()
    {
        // Arrange
        var sale = CreateTestSale();
        await _repository.CreateAsync(sale);

        // Act
        var retrieved = await _repository.GetByIdAsync(sale.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(sale.Id);
        retrieved.Items.Should().HaveCount(sale.Items.Count);
        retrieved.TotalAmount.Should().BeGreaterThan(0);
    }

    [Fact(DisplayName = "Given non-existent sale id When getting by id Then should return null")]
    public async Task GetByIdAsync_NonExistentId_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var retrieved = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        retrieved.Should().BeNull();
    }

    [Fact(DisplayName = "Given multiple sales When getting all Then should return all sales")]
    public async Task GetAllAsync_MultipleSales_ShouldReturnAllSales()
    {
        // Arrange
        var sale1 = CreateTestSale();
        var sale2 = CreateTestSale();
        await _repository.CreateAsync(sale1);
        await _repository.CreateAsync(sale2);

        // Act
        var allSales = await _repository.GetAllAsync();

        // Assert
        allSales.Should().HaveCountGreaterThanOrEqualTo(2);
        allSales.Should().Contain(s => s.Id == sale1.Id);
        allSales.Should().Contain(s => s.Id == sale2.Id);
    }

    [Fact(DisplayName = "Given existing sale When updating Then sale should be updated in database")]
    public async Task UpdateAsync_ExistingSale_ShouldUpdateInDatabase()
    {
        // Arrange
        var sale = CreateTestSale();
        await _repository.CreateAsync(sale);
        sale.Complete();

        // Act
        var updated = await _repository.UpdateAsync(sale);

        // Assert
        updated.Status.Should().Be(Domain.Enums.SaleStatus.Completed);

        var retrieved = await _repository.GetByIdAsync(sale.Id);
        retrieved!.Status.Should().Be(Domain.Enums.SaleStatus.Completed);
    }

    [Fact(DisplayName = "Given existing sale id When deleting Then sale should be removed from database")]
    public async Task DeleteAsync_ExistingSaleId_ShouldRemoveFromDatabase()
    {
        // Arrange
        var sale = CreateTestSale();
        await _repository.CreateAsync(sale);

        // Act
        var deleted = await _repository.DeleteAsync(sale.Id);

        // Assert
        deleted.Should().BeTrue();

        var retrieved = await _repository.GetByIdAsync(sale.Id);
        retrieved.Should().BeNull();
    }

    [Fact(DisplayName = "Given non-existent sale id When deleting Then should return false")]
    public async Task DeleteAsync_NonExistentId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var deleted = await _repository.DeleteAsync(nonExistentId);

        // Assert
        deleted.Should().BeFalse();
    }

    private static Domain.Entities.Sale CreateTestSale()
    {
        var faker = new Bogus.Faker();
        var saleNumber = $"S-{faker.Random.Number(1000, 9999)}";
        var items = new List<Domain.Entities.SaleItem>
        {
            new Domain.Entities.SaleItem(
                Guid.NewGuid(),
                faker.Commerce.ProductName(),
                5,
                100m
            )
        };

        return new Domain.Entities.Sale(
            saleNumber,
            DateTime.UtcNow,
            Guid.NewGuid(),
            faker.Person.FullName,
            Guid.NewGuid(),
            faker.Company.CompanyName(),
            items
        );
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}

