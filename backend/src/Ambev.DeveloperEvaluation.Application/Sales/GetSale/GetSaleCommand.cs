using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Command to retrieve a sale by Id.
/// </summary>
public record GetSaleCommand(Guid Id) : IRequest<GetSaleResult>;


