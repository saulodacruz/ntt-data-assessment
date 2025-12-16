using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for Sale entities using the Bogus library.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// </summary>
    public static Sale GenerateValidSale()
    {
        var faker = new Faker();
        var saleNumber = $"S-{faker.Random.Number(1000, 9999)}";
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        var items = new List<SaleItem>
        {
            new SaleItem(
                Guid.NewGuid(),
                faker.Commerce.ProductName(),
                faker.Random.Int(1, 20),
                faker.Random.Decimal(10, 1000)
            )
        };

        return new Sale(
            saleNumber,
            DateTime.UtcNow,
            customerId,
            faker.Person.FullName,
            branchId,
            faker.Company.CompanyName(),
            items
        );
    }

    /// <summary>
    /// Generates a Sale with multiple items.
    /// </summary>
    public static Sale GenerateSaleWithMultipleItems(int itemCount = 3)
    {
        var faker = new Faker();
        var saleNumber = $"S-{faker.Random.Number(1000, 9999)}";
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        var items = new List<SaleItem>();
        for (int i = 0; i < itemCount; i++)
        {
            items.Add(new SaleItem(
                Guid.NewGuid(),
                faker.Commerce.ProductName(),
                faker.Random.Int(1, 20),
                faker.Random.Decimal(10, 1000)
            ));
        }

        return new Sale(
            saleNumber,
            DateTime.UtcNow,
            customerId,
            faker.Person.FullName,
            branchId,
            faker.Company.CompanyName(),
            items
        );
    }

    /// <summary>
    /// Generates a SaleItem with specific quantity for discount testing.
    /// </summary>
    public static SaleItem GenerateSaleItemWithQuantity(int quantity, decimal unitPrice = 100m)
    {
        var faker = new Faker();
        return new SaleItem(
            Guid.NewGuid(),
            faker.Commerce.ProductName(),
            quantity,
            unitPrice
        );
    }
}

