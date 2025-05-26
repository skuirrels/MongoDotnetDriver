using EFTest.Domain.Entities;
using EFTest.Domain.ValueObjects;

namespace EFTest.Domain.DomainServices;

public interface IOrderDomainService
{
    Task<bool> CanCreateOrderAsync(CustomerName customerName, CancellationToken cancellationToken = default);
    Task<Money> CalculateOrderDiscountAsync(Order order, CancellationToken cancellationToken = default);
    Task ValidateOrderBusinessRulesAsync(Order order, CancellationToken cancellationToken = default);
}
