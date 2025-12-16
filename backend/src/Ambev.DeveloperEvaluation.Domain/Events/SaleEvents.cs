using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a sale is created.
/// </summary>
public class SaleCreatedEvent
{
    public Sale Sale { get; }

    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
    }
}

/// <summary>
/// Raised when a sale is modified (e.g., completed).
/// </summary>
public class SaleModifiedEvent
{
    public Sale Sale { get; }

    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale;
    }
}

/// <summary>
/// Raised when a sale is cancelled.
/// </summary>
public class SaleCancelledEvent
{
    public Sale Sale { get; }

    public SaleCancelledEvent(Sale sale)
    {
        Sale = sale;
    }
}

/// <summary>
/// Raised when an item inside a sale is cancelled.
/// </summary>
public class ItemCancelledEvent
{
    public Sale Sale { get; }
    public SaleItem Item { get; }

    public ItemCancelledEvent(Sale sale, SaleItem item)
    {
        Sale = sale;
        Item = item;
    }
}


