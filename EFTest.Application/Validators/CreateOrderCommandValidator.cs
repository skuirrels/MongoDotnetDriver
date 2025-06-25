using EFTest.Application.Commands;
using FluentValidation;
using EFTest.Application.DTOs;
namespace EFTest.Application.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName.Value)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .Length(1, 100)
            .WithMessage("Customer name must be between 1 and 100 characters")
            .Matches(@"^[a-zA-Z\s\-\.]+$")
            .WithMessage("Customer name can only contain letters, spaces, hyphens, and periods");

        RuleFor(x => x.OrderDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Order date cannot be more than 1 day in the future")
            .When(x => x.OrderDate.HasValue);

        RuleFor(x => x.OrderLines)
            .NotEmpty()
            .WithMessage("Order must contain at least one order line")
            .Must(orderLines => orderLines.Count <= 50)
            .WithMessage("Order cannot contain more than 50 order lines");

        RuleForEach(x => x.OrderLines)
            .SetValidator(new CreateOrderLineValidator());
    }
}

public class CreateOrderLineValidator : AbstractValidator<CreateOrderLineDto>
{
    public CreateOrderLineValidator()
    {
        RuleFor(x => x.ProductName.Value)
            .NotEmpty()
            .WithMessage("Product name is required")
            .Length(1, 200)
            .WithMessage("Product name must be between 1 and 200 characters")
            .Matches(@"^[a-zA-Z0-9\s\-\.\(\)\""/]+$")
            .WithMessage("Product name contains invalid characters");

        RuleFor(x => x.Quantity.Value)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantity cannot exceed 1000");

        RuleFor(x => x.UnitPrice.Amount)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than 0")
            .LessThan(100000)
            .WithMessage("Unit price cannot exceed $100,000");

        RuleFor(x => x.UnitPrice.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be 3 characters")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Currency must be in ISO format (e.g., USD, EUR)");
    }
}
