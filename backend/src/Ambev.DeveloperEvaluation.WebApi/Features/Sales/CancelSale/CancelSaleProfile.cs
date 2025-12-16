using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

/// <summary>
/// Profile for mapping CancelSale feature requests to commands.
/// </summary>
public class CancelSaleProfile : Profile
{
    public CancelSaleProfile()
    {
        CreateMap<CancelSaleRequest, Application.Sales.CancelSale.CancelSaleCommand>()
            .ConstructUsing(req => new Application.Sales.CancelSale.CancelSaleCommand(req.Id));
        CreateMap<Application.Sales.CancelSale.CancelSaleResult, CancelSaleResponse>();
    }
}

