using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Request body for creating a sale.
/// </summary>
public class CreateSaleRequest
{
    [Required]
    public string SaleNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    public Guid BranchId { get; set; }

    [Required]
    public string BranchName { get; set; } = string.Empty;

    [Required]
    public IList<CreateSaleItemRequest> Items { get; set; } = new List<CreateSaleItemRequest>();
}

/// <summary>
/// Represents an item of a sale in the request.
/// </summary>
public class CreateSaleItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public string ProductDescription { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }
}


