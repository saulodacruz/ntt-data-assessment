using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale in the system.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Sequential sale number.
    /// </summary>
    public string SaleNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; private set; }

    /// <summary>
    /// External identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Denormalized customer name.
    /// </summary>
    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>
    /// External identifier of the branch.
    /// </summary>
    public Guid BranchId { get; private set; }

    /// <summary>
    /// Denormalized branch name.
    /// </summary>
    public string BranchName { get; private set; } = string.Empty;

    /// <summary>
    /// Total sale amount.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Indicates whether the sale was cancelled.
    /// </summary>
    public bool IsCancelled => Status == SaleStatus.Cancelled;

    /// <summary>
    /// Status of the sale.
    /// </summary>
    public SaleStatus Status { get; private set; }

    /// <summary>
    /// Items of this sale.
    /// </summary>
    public ICollection<SaleItem> Items { get; private set; } = new List<SaleItem>();

    private readonly List<object> _domainEvents = new();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    private Sale() { }

    public Sale(
        string saleNumber,
        DateTime saleDate,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName,
        IEnumerable<SaleItem> items)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new DomainException("Sale number is required.");

        if (customerId == Guid.Empty)
            throw new DomainException("CustomerId is required.");

        if (branchId == Guid.Empty)
            throw new DomainException("BranchId is required.");

        if (items == null || !items.Any())
            throw new DomainException("Sale must have at least one item.");

        SaleNumber = saleNumber;
        SaleDate = saleDate == default ? DateTime.UtcNow : saleDate;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        Status = SaleStatus.Pending;

        foreach (var item in items)
        {
            Items.Add(item);
        }

        RecalculateTotal();

        AddDomainEvent(new SaleCreatedEvent(this));
    }

    /// <summary>
    /// Marks the sale as completed.
    /// </summary>
    public void Complete()
    {
        if (IsCancelled)
            throw new DomainException("Cannot complete a cancelled sale.");

        Status = SaleStatus.Completed;
        AddDomainEvent(new SaleModifiedEvent(this));
    }

    /// <summary>
    /// Cancels the sale.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            return;

        Status = SaleStatus.Cancelled;
        AddDomainEvent(new SaleCancelledEvent(this));
    }

    /// <summary>
    /// Cancels an item in the sale by its identifier.
    /// </summary>
    public void CancelItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException("Item not found in sale.");

        item.Cancel();
        RecalculateTotal();
        AddDomainEvent(new ItemCancelledEvent(this, item));
    }

    /// <summary>
    /// Recalculates the total amount of the sale considering non-cancelled items.
    /// </summary>
    private void RecalculateTotal()
    {
        TotalAmount = Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
    }

    private void AddDomainEvent(object @event)
    {
        _domainEvents.Add(@event);
    }
}


