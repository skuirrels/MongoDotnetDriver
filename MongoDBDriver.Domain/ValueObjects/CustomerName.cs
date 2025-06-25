using System.Text.RegularExpressions;

namespace MongoDBDriver.Domain.ValueObjects;

public class CustomerName : ValueObject
{
    private static readonly Regex ValidNamePattern = new(@"^[a-zA-Z\s\-\.]+$", RegexOptions.Compiled);
    
    public string Value { get; private set; }

    private CustomerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Customer name cannot be null or empty", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Customer name cannot exceed 100 characters", nameof(value));

        if (!ValidNamePattern.IsMatch(value))
            throw new ArgumentException("Customer name can only contain letters, spaces, hyphens, and periods", nameof(value));

        Value = value.Trim();
    }

    public static CustomerName Create(string value)
    {
        return new CustomerName(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(CustomerName customerName)
    {
        return customerName.Value;
    }

    public static explicit operator CustomerName(string value)
    {
        return Create(value);
    }
}
