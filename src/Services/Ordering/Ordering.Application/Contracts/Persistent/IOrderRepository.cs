using Ordering.Domain.Entities;

namespace Ordering.Application.Contracts.Persistent;

public interface IOrderRepository : IAsyncRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
}