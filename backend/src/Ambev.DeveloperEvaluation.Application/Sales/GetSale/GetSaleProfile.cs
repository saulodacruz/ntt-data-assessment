using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// AutoMapper profile for GetSale feature.
/// </summary>
public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        // Map Sale to GetSaleResult - using ConstructUsing to ensure all properties are mapped correctly
        CreateMap<Sale, GetSaleResult>()
            .ConstructUsing(src => new GetSaleResult
            {
                Id = src.Id,
                SaleNumber = src.SaleNumber,
                SaleDate = src.SaleDate,
                CustomerId = src.CustomerId,
                CustomerName = src.CustomerName,
                BranchId = src.BranchId,
                BranchName = src.BranchName,
                TotalAmount = src.TotalAmount,
                Status = src.Status,
                IsCancelled = src.IsCancelled
            });
    }
}


