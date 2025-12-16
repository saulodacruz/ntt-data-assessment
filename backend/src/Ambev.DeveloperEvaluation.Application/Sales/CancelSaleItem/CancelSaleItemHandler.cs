using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Handler for processing CancelSaleItemCommand requests.
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item == null)
            throw new KeyNotFoundException($"Item with ID {request.ItemId} not found in sale {request.SaleId}");

        var productDescription = item.ProductDescription;
        sale.CancelItem(request.ItemId);
        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Log ItemCancelled event
        _logger.LogInformation("ItemCancelled event: Item {ItemId} (Product: {ProductDescription}) from Sale {SaleNumber} (Id: {SaleId}) was cancelled",
            request.ItemId, productDescription, updatedSale.SaleNumber, updatedSale.Id);

        return new CancelSaleItemResult
        {
            SaleId = updatedSale.Id,
            ItemId = request.ItemId,
            ProductDescription = productDescription,
            TotalAmount = updatedSale.TotalAmount
        };
    }
}

