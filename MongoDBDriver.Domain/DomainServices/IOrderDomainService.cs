using MongoDBDriver.Domain.Entities;
using MongoDBDriver.Domain.ValueObjects;

namespace MongoDBDriver.Domain.DomainServices;

public interface IOrderDomainService
{
    Task<bool> CanCreateOrderAsync(CustomerName customerName, CancellationToken cancellationToken = default);
    Task<Money> CalculateOrderDiscountAsync(Order order, CancellationToken cancellationToken = default);
    Task ValidateOrderBusinessRulesAsync(Order order, CancellationToken cancellationToken = default);
}
