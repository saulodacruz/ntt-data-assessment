using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover business rules, discount calculations, and status changes.
/// </summary>
public class SaleTests
{
    [Fact(DisplayName = "Given valid sale data When creating sale Then sale should be created with Pending status")]
    public void Given_ValidSaleData_When_CreatingSale_Then_ShouldBeCreatedWithPendingStatus()
    {
        // Arrange & Act
        var sale = SaleTestData.GenerateValidSale();

        // Assert
        sale.Should().NotBeNull();
        sale.Status.Should().Be(SaleStatus.Pending);
        sale.IsCancelled.Should().BeFalse();
    }

    [Fact(DisplayName = "Given sale When completing Then status should be Completed")]
    public void Given_Sale_When_Completing_Then_StatusShouldBeCompleted()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Complete();

        // Assert
        sale.Status.Should().Be(SaleStatus.Completed);
    }

    [Fact(DisplayName = "Given cancelled sale When completing Then should throw DomainException")]
    public void Given_CancelledSale_When_Completing_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act
        var act = () => sale.Complete();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot complete a cancelled sale.");
    }

    [Fact(DisplayName = "Given sale When cancelling Then status should be Cancelled")]
    public void Given_Sale_When_Cancelling_Then_StatusShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Given sale with item When cancelling item Then item should be cancelled and total recalculated")]
    public void Given_SaleWithItem_When_CancellingItem_Then_ItemShouldBeCancelledAndTotalRecalculated()
    {
        // Arrange
        var sale = SaleTestData.GenerateSaleWithMultipleItems(2);
        var originalTotal = sale.TotalAmount;
        var itemToCancel = sale.Items.First();

        // Act
        sale.CancelItem(itemToCancel.Id);

        // Assert
        itemToCancel.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().BeLessThan(originalTotal);
    }

    [Fact(DisplayName = "Given sale When cancelling non-existent item Then should throw DomainException")]
    public void Given_Sale_When_CancellingNonExistentItem_Then_ShouldThrowDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var nonExistentItemId = Guid.NewGuid();

        // Act
        var act = () => sale.CancelItem(nonExistentItemId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Item not found in sale.");
    }

    [Fact(DisplayName = "Given sale with empty items When creating Then should throw DomainException")]
    public void Given_EmptyItems_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        // Arrange
        var faker = new Bogus.Faker();
        var saleNumber = $"S-{faker.Random.Number(1000, 9999)}";
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        // Act
        var act = () => new Sale(
            saleNumber,
            DateTime.UtcNow,
            customerId,
            faker.Person.FullName,
            branchId,
            faker.Company.CompanyName(),
            new List<SaleItem>()
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Sale must have at least one item.");
    }

    [Fact(DisplayName = "Given sale When total is calculated Then should sum all non-cancelled items")]
    public void Given_Sale_When_TotalCalculated_Then_ShouldSumAllNonCancelledItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateSaleWithMultipleItems(3);
        var expectedTotal = sale.Items.Sum(i => i.TotalAmount);

        // Assert
        sale.TotalAmount.Should().Be(expectedTotal);
    }
}

