using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the CancelSaleItemHandler class.
/// </summary>
public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleItemHandler> _logger;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CancelSaleItemHandler>>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given existing sale and item ids When cancelling item Then returns cancelled item result")]
    public async Task Handle_ExistingSaleAndItemIds_ReturnsCancelledItemResult()
    {
        // Given
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new CancelSaleItemCommand(saleId, itemId);
        var sale = SaleTestData.GenerateSaleWithMultipleItems(2);
        sale.Id = saleId;
        var item = sale.Items.First();
        item.Id = itemId;

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.SaleId.Should().Be(saleId);
        result.ItemId.Should().Be(itemId);
        item.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale id When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSaleId_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new CancelSaleItemCommand(saleId, itemId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }

    [Fact(DisplayName = "Given non-existent item id When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentItemId_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var command = new CancelSaleItemCommand(saleId, itemId);
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = saleId;

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Item with ID {itemId} not found in sale {saleId}");
    }
}

