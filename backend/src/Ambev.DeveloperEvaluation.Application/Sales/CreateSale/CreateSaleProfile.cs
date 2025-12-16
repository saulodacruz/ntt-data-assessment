using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// AutoMapper profile for CreateSale feature.
/// </summary>
public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ConstructUsing((command, ctx) =>
            {
                var items = command.Items.Select(i =>
                    new SaleItem(i.ProductId, i.ProductDescription, i.Quantity, i.UnitPrice));

                return new Sale(
                    command.SaleNumber,
                    command.SaleDate,
                    command.CustomerId,
                    command.CustomerName,
                    command.BranchId,
                    command.BranchName,
                    items);
            })
            .ForMember(dest => dest.Items, opt => opt.Ignore()); // Items are created in ConstructUsing

        CreateMap<Sale, CreateSaleResult>();
    }
}


