using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

/// <summary>
/// Integration tests for CreateSaleHandler using real repository and database.
/// These tests verify the complete flow from command to database persistence.
/// </summary>
public class CreateSaleHandlerIntegrationTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly ISaleRepository _repository;
    private readonly CreateSaleHandler _handler;
    private readonly IMapper _mapper;

    public CreateSaleHandlerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
        
        var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Ambev.DeveloperEvaluation.Application.Sales.CreateSale.CreateSaleProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        
        var logger = new LoggerFactory().CreateLogger<CreateSaleHandler>();
        _handler = new CreateSaleHandler(_repository, _mapper, logger);
    }

    [Fact(DisplayName = "Given valid create sale command When handling Then should create sale with correct discount rules")]
    public async Task Handle_ValidCommand_ShouldCreateSaleWithCorrectDiscounts()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "S-1001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Centro",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto A",
                    Quantity = 5, // Should get 10% discount
                    UnitPrice = 100m
                },
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto B",
                    Quantity = 15, // Should get 20% discount
                    UnitPrice = 50m
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        
        var savedSale = await _repository.GetByIdAsync(result.Id);
        savedSale.Should().NotBeNull();
        savedSale!.Items.Should().HaveCount(2);
        
        // Verify discount rules
        var item1 = savedSale.Items.First(i => i.Quantity == 5);
        item1.DiscountPercentage.Should().Be(10);
        item1.TotalAmount.Should().Be(450m); // (5 * 100) - 10% = 450
        
        var item2 = savedSale.Items.First(i => i.Quantity == 15);
        item2.DiscountPercentage.Should().Be(20);
        item2.TotalAmount.Should().Be(600m); // (15 * 50) - 20% = 600
    }

    [Fact(DisplayName = "Given command with quantity above 20 When handling Then should throw validation exception")]
    public async Task Handle_QuantityAbove20_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "S-1002",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Centro",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductDescription = "Produto A",
                    Quantity = 21, // Above limit
                    UnitPrice = 100m
                }
            }
        };

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}

