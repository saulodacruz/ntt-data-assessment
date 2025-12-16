using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// External identifier of the product.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Denormalized product description.
    /// </summary>
    public string ProductDescription { get; private set; } = string.Empty;

    /// <summary>
    /// Quantity of the product in the sale.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Discount percentage applied to this item (0-100).
    /// </summary>
    public decimal DiscountPercentage { get; private set; }

    /// <summary>
    /// Total amount for this item after discount.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Indicates whether the item is cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Identifier of the parent sale.
    /// </summary>
    public Guid SaleId { get; private set; }

    public Sale? Sale { get; private set; }

    private SaleItem() { }

    public SaleItem(Guid productId, string productDescription, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new DomainException("It's not possible to sell above 20 identical items.");

        if (unitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        ProductId = productId;
        ProductDescription = productDescription;
        UnitPrice = unitPrice;

        ApplyQuantityRules(quantity);
        RecalculateTotal();
    }

    /// <summary>
    /// Applies business rules based on quantity, setting discount percentage.
    /// </summary>
    /// <param name="quantity">Quantity of items.</param>
    private void ApplyQuantityRules(int quantity)
    {
        if (quantity < 4)
        {
            // Purchases below 4 items cannot have a discount
            DiscountPercentage = 0;
        }
        else if (quantity >= 4 && quantity < 10)
        {
            // Purchases above 4 identical items have a 10% discount
            DiscountPercentage = 10;
        }
        else if (quantity <= 20)
        {
            // Purchases between 10 and 20 identical items have a 20% discount
            DiscountPercentage = 20;
        }

        Quantity = quantity;
    }

    /// <summary>
    /// Recalculates total amount for this item.
    /// </summary>
    private void RecalculateTotal()
    {
        var gross = UnitPrice * Quantity;
        var discount = gross * (DiscountPercentage / 100m);
        TotalAmount = decimal.Round(gross - discount, 2);
    }

    /// <summary>
    /// Cancels this item.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
    }
}


