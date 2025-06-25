using System.Text.RegularExpressions;

namespace MongoDBDriver.Domain.ValueObjects;

public class ProductName : ValueObject
{
    private static readonly Regex ValidNamePattern = new(@"^[a-zA-Z0-9\s\-\.\(\)\""/]+$", RegexOptions.Compiled);
    
    public string Value { get; private set; }

    private ProductName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Product name cannot be null or empty", nameof(value));

        if (value.Length > 200)
            throw new ArgumentException("Product name cannot exceed 200 characters", nameof(value));

        if (!ValidNamePattern.IsMatch(value))
            throw new ArgumentException("Product name contains invalid characters", nameof(value));

        Value = value.Trim();
    }

    public static ProductName Create(string value)
    {
        return new ProductName(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(ProductName productName)
    {
        return productName.Value;
    }

    public static explicit operator ProductName(string value)
    {
        return Create(value);
    }
}
