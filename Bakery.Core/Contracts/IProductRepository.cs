﻿using Bakery.Core.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Bakery.Core.Contracts
{
  public interface IProductRepository
  {
    Task<int> GetCountAsync();
    Task AddRangeAsync(IEnumerable<Product> products);
        Task<Product[]> GetAllAsync();
        void Update(Product product);
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
    }
}