using EFTest.Application.DTOs;
using EFTest.Domain.Entities;

namespace EFTest.Application.Services;

public interface IOrderMappingService
{
    OrderDto MapToDto(Order order);
    OrderLineDto MapToDto(OrderLine orderLine);
}
