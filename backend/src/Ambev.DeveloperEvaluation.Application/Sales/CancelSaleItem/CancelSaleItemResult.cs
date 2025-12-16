namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Result returned after cancelling a sale item.
/// </summary>
public class CancelSaleItemResult
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public string ProductDescription { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

