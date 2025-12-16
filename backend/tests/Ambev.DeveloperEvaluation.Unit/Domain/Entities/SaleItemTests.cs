using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover discount rules based on quantity and business validations.
/// </summary>
public class SaleItemTests
{
    [Fact(DisplayName = "Given quantity less than 4 When creating item Then discount should be 0%")]
    public void Given_QuantityLessThan4_When_CreatingItem_Then_DiscountShouldBeZero()
    {
        // Arrange & Act
        var item = SaleTestData.GenerateSaleItemWithQuantity(3, 100m);

        // Assert
        item.DiscountPercentage.Should().Be(0);
        item.TotalAmount.Should().Be(300m); // 3 * 100 = 300
    }

    [Fact(DisplayName = "Given quantity between 4 and 9 When creating item Then discount should be 10%")]
    public void Given_QuantityBetween4And9_When_CreatingItem_Then_DiscountShouldBe10Percent()
    {
        // Arrange & Act
        var item = SaleTestData.GenerateSaleItemWithQuantity(5, 100m);

        // Assert
        item.DiscountPercentage.Should().Be(10);
        item.TotalAmount.Should().Be(450m); // (5 * 100) - 10% = 500 - 50 = 450
    }

    [Fact(DisplayName = "Given quantity between 10 and 20 When creating item Then discount should be 20%")]
    public void Given_QuantityBetween10And20_When_CreatingItem_Then_DiscountShouldBe20Percent()
    {
        // Arrange & Act
        var item = SaleTestData.GenerateSaleItemWithQuantity(15, 100m);

        // Assert
        item.DiscountPercentage.Should().Be(20);
        item.TotalAmount.Should().Be(1200m); // (15 * 100) - 20% = 1500 - 300 = 1200
    }

    [Fact(DisplayName = "Given quantity exactly 4 When creating item Then discount should be 10%")]
    public void Given_QuantityExactly4_When_CreatingItem_Then_DiscountShouldBe10Percent()
    {
        // Arrange & Act
        var item = SaleTestData.GenerateSaleItemWithQuantity(4, 100m);

        // Assert
        item.DiscountPercentage.Should().Be(10);
        item.TotalAmount.Should().Be(360m); // (4 * 100) - 10% = 400 - 40 = 360
    }

    [Fact(DisplayName = "Given quantity exactly 10 When creating item Then discount should be 20%")]
    public void Given_QuantityExactly10_When_CreatingItem_Then_DiscountShouldBe20Percent()
    {
        // Arrange & Act
        var item = SaleTestData.GenerateSaleItemWithQuantity(10, 100m);

        // Assert
        item.DiscountPercentage.Should().Be(20);
        item.TotalAmount.Should().Be(800m); // (10 * 100) - 20% = 1000 - 200 = 800
    }

    [Fact(DisplayName = "Given quantity exactly 20 When creating item Then discount should be 20%")]
    public void Given_QuantityExactly20_When_CreatingItem_Then_DiscountShouldBe20Percent()
    {
        // Arrange & Act
        var item = SaleTestData.GenerateSaleItemWithQuantity(20, 100m);

        // Assert
        item.DiscountPercentage.Should().Be(20);
        item.TotalAmount.Should().Be(1600m); // (20 * 100) - 20% = 2000 - 400 = 1600
    }

    [Fact(DisplayName = "Given quantity greater than 20 When creating item Then should throw DomainException")]
    public void Given_QuantityGreaterThan20_When_CreatingItem_Then_ShouldThrowDomainException()
    {
        // Arrange
        var faker = new Bogus.Faker();

        // Act
        var act = () => new SaleItem(
            Guid.NewGuid(),
            faker.Commerce.ProductName(),
            21, // > 20
            100m
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("It's not possible to sell above 20 identical items.");
    }

    [Fact(DisplayName = "Given quantity zero When creating item Then should throw DomainException")]
    public void Given_QuantityZero_When_CreatingItem_Then_ShouldThrowDomainException()
    {
        // Arrange
        var faker = new Bogus.Faker();

        // Act
        var act = () => new SaleItem(
            Guid.NewGuid(),
            faker.Commerce.ProductName(),
            0,
            100m
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Given negative quantity When creating item Then should throw DomainException")]
    public void Given_NegativeQuantity_When_CreatingItem_Then_ShouldThrowDomainException()
    {
        // Arrange
        var faker = new Bogus.Faker();

        // Act
        var act = () => new SaleItem(
            Guid.NewGuid(),
            faker.Commerce.ProductName(),
            -1,
            100m
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be greater than zero.");
    }

    [Fact(DisplayName = "Given zero unit price When creating item Then should throw DomainException")]
    public void Given_ZeroUnitPrice_When_CreatingItem_Then_ShouldThrowDomainException()
    {
        // Arrange
        var faker = new Bogus.Faker();

        // Act
        var act = () => new SaleItem(
            Guid.NewGuid(),
            faker.Commerce.ProductName(),
            5,
            0m
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Unit price must be greater than zero.");
    }

    [Fact(DisplayName = "Given item When cancelling Then IsCancelled should be true")]
    public void Given_Item_When_Cancelling_Then_IsCancelledShouldBeTrue()
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(5, 100m);

        // Act
        item.Cancel();

        // Assert
        item.IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Given cancelled item When cancelling again Then should not throw exception")]
    public void Given_CancelledItem_When_CancellingAgain_Then_ShouldNotThrowException()
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(5, 100m);
        item.Cancel();

        // Act
        var act = () => item.Cancel();

        // Assert
        act.Should().NotThrow();
        item.IsCancelled.Should().BeTrue();
    }
}

