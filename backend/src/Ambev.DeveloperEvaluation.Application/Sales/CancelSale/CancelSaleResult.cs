namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Result returned after cancelling a sale.
/// </summary>
public class CancelSaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public Domain.Enums.SaleStatus Status { get; set; }
}

