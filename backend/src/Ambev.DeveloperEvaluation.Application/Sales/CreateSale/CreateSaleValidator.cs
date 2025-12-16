using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.BranchId)
            .NotEmpty();

        RuleFor(x => x.BranchName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item.");

        RuleForEach(x => x.Items).SetValidator(new CreateSaleItemValidator());
    }
}

/// <summary>
/// Validator for CreateSaleItemDto.
/// </summary>
public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemDto>
{
    public CreateSaleItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.ProductDescription)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .LessThanOrEqualTo(20);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);
    }
}


