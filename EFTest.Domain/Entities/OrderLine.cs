using EFTest.Domain.ValueObjects;

namespace EFTest.Domain.Entities;

public class OrderLine : Entity
{
    public ProductName ProductName { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }

    private OrderLine(ProductName productName, Quantity quantity, Money unitPrice)
    {
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
    }

    public static OrderLine Create(ProductName productName, Quantity quantity, Money unitPrice)
    {
        return new OrderLine(productName, quantity, unitPrice);
    }

    public Money CalculateLineTotal()
    {
        return UnitPrice.Multiply(Quantity.Value);
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        if (newQuantity == null)
            throw new ArgumentNullException(nameof(newQuantity));

        Quantity = newQuantity;
    }

    public void UpdateUnitPrice(Money newUnitPrice)
    {
        if (newUnitPrice == null)
            throw new ArgumentNullException(nameof(newUnitPrice));

        if (newUnitPrice.Currency != UnitPrice.Currency)
            throw new InvalidOperationException($"Cannot change currency from {UnitPrice.Currency} to {newUnitPrice.Currency}");

        UnitPrice = newUnitPrice;
    }
}
