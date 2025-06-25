namespace MongoDBDriver.Domain.ValueObjects;

public class Quantity : ValueObject
{
    public int Value { get; private set; }

    private Quantity(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(value));

        if (value > 1000)
            throw new ArgumentException("Quantity cannot exceed 1000", nameof(value));

        Value = value;
    }

    public static Quantity Create(int value)
    {
        return new Quantity(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator int(Quantity quantity)
    {
        return quantity.Value;
    }

    public static explicit operator Quantity(int value)
    {
        return Create(value);
    }
}
