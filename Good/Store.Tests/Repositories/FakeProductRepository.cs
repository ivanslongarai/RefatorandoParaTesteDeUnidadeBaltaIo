using System;
using System.Collections.Generic;
using Store.Domain.Entities;
using Store.Domain.Repositories.Interfaces;

namespace Store.Tests.Repositories
{
    public class FakeProductRepository : IProductRepository
    {
        public IEnumerable<Product> Get(IEnumerable<Guid> ids)
        {
            IList<Product> products = new List<Product>();
            products.Add(new Product("Produto 01", 100, true));
            products.Add(new Product("Produto 02", 100, true));
            products.Add(new Product("Produto 03", 100, true));
            products.Add(new Product("Produto 04", 100, false));
            products.Add(new Product("Produto 05", 100, false));

            return products;
        }
    }
}