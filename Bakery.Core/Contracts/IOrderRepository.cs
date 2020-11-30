using Bakery.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bakery.Core.Contracts
{
    public interface IOrderRepository
    {
        Task<int> GetCountAsync();
        Task AddRangeAsync(IEnumerable<Order> orders);
    }
}