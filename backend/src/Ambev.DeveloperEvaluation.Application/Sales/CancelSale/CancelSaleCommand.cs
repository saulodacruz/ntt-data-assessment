using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command to cancel a sale.
/// </summary>
public class CancelSaleCommand : IRequest<CancelSaleResult>
{
    public Guid Id { get; set; }

    public CancelSaleCommand(Guid id)
    {
        Id = id;
    }
}

