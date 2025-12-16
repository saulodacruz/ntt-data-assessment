using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for CreateSaleCommand using the Bogus library.
/// </summary>
public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleCommand> CreateSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.SaleNumber, f => $"S-{f.Random.Number(1000, 9999)}")
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.BranchId, f => Guid.NewGuid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => new List<CreateSaleItemDto>
        {
            new CreateSaleItemDto
            {
                ProductId = Guid.NewGuid(),
                ProductDescription = f.Commerce.ProductName(),
                Quantity = f.Random.Int(1, 20),
                UnitPrice = f.Random.Decimal(10, 1000)
            }
        });

    /// <summary>
    /// Generates a valid CreateSaleCommand with randomized data.
    /// </summary>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return CreateSaleCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a CreateSaleCommand with multiple items.
    /// </summary>
    public static CreateSaleCommand GenerateCommandWithMultipleItems(int itemCount = 3)
    {
        var faker = new Faker();
        var command = new CreateSaleCommand
        {
            SaleNumber = $"S-{faker.Random.Number(1000, 9999)}",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Person.FullName,
            BranchId = Guid.NewGuid(),
            BranchName = faker.Company.CompanyName(),
            Items = new List<CreateSaleItemDto>()
        };

        for (int i = 0; i < itemCount; i++)
        {
            command.Items.Add(new CreateSaleItemDto
            {
                ProductId = Guid.NewGuid(),
                ProductDescription = faker.Commerce.ProductName(),
                Quantity = faker.Random.Int(1, 20),
                UnitPrice = faker.Random.Decimal(10, 1000)
            });
        }

        return command;
    }
}

