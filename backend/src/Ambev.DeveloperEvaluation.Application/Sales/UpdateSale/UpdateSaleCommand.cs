using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command to update an existing sale (mark as completed).
/// </summary>
public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    public Guid Id { get; set; }

    public UpdateSaleCommand(Guid id)
    {
        Id = id;
    }
}

