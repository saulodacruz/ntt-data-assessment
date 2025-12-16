using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the CancelSaleHandler class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given existing sale id When cancelling sale Then returns cancelled sale result")]
    public async Task Handle_ExistingSaleId_ReturnsCancelledSaleResult()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = saleId;

        var result = new CancelSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Status = SaleStatus.Cancelled
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CancelSaleResult>(sale).Returns(result);

        // When
        var cancelSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        cancelSaleResult.Should().NotBeNull();
        cancelSaleResult.Id.Should().Be(saleId);
        sale.Status.Should().Be(SaleStatus.Cancelled);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale id When cancelling sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSaleId_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }
}

