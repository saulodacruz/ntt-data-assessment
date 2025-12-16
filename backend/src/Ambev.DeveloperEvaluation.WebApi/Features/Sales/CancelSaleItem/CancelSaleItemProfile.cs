using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

/// <summary>
/// Profile for mapping CancelSaleItem feature requests to commands.
/// </summary>
public class CancelSaleItemProfile : Profile
{
    public CancelSaleItemProfile()
    {
        CreateMap<CancelSaleItemRequest, Application.Sales.CancelSaleItem.CancelSaleItemCommand>()
            .ConstructUsing(req => new Application.Sales.CancelSaleItem.CancelSaleItemCommand(req.SaleId, req.ItemId));
        CreateMap<Application.Sales.CancelSaleItem.CancelSaleItemResult, CancelSaleItemResponse>();
    }
}

