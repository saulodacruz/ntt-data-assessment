namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

/// <summary>
/// API response model for CancelSaleItem operation.
/// </summary>
public class CancelSaleItemResponse
{
    /// <summary>
    /// The unique identifier of the sale.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// The unique identifier of the cancelled item.
    /// </summary>
    public Guid ItemId { get; set; }

    /// <summary>
    /// The product description of the cancelled item.
    /// </summary>
    public string ProductDescription { get; set; } = string.Empty;

    /// <summary>
    /// The updated total amount of the sale after cancelling the item.
    /// </summary>
    public decimal TotalAmount { get; set; }
}

