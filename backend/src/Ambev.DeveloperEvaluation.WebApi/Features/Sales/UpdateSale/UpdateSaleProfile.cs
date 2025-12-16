using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Profile for mapping UpdateSale feature requests to commands.
/// </summary>
public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, Application.Sales.UpdateSale.UpdateSaleCommand>()
            .ConstructUsing(req => new Application.Sales.UpdateSale.UpdateSaleCommand(req.Id));
        CreateMap<Application.Sales.UpdateSale.UpdateSaleResult, UpdateSaleResponse>();
    }
}

