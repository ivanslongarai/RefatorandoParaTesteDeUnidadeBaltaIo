using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Store.Domain.Entities;
using Store.Domain.Queries;

namespace Store.Tests.Queries
{
    [TestClass]
    public class ProductQueriesTests
    {
        private IList<Product> _products;

        public ProductQueriesTests()
        {
            _products = new List<Product>();
            _products.Add(new Product("Produto 001", 100, true));
            _products.Add(new Product("Produto 002", 200, true));
            _products.Add(new Product("Produto 003", 300, true));
            _products.Add(new Product("Produto 004", 400, false));
            _products.Add(new Product("Produto 005", 500, false));
            _products.Add(new Product("Produto 005", 600, false));
        }

        [TestMethod]
        [TestCategory("Queries")]
        public void GivenGetActiveProductsShouldReturnCount3()
        {
            var result = _products.AsQueryable().Where(ProductQueries.GetActiveProducts());
            Assert.AreEqual(result.Count(), 3);
        }

        [TestMethod]
        [TestCategory("Queries")]
        public void GivenGetInactiveProductsShouldReturnCount3()
        {
            var result = _products.AsQueryable().Where(ProductQueries.GetInactiveProducts());
            Assert.AreEqual(result.Count(), 3);
        }
    }
}