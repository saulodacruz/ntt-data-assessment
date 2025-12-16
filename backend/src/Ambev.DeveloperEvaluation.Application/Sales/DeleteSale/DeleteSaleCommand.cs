using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command to delete a sale by Id.
/// </summary>
public record DeleteSaleCommand(Guid Id) : IRequest<DeleteSaleResult>;


