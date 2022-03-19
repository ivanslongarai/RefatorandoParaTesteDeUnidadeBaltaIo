using System;
using Store.Domain.Entities;
using Store.Domain.Repositories.Interfaces;

namespace Store.Tests.Repositories
{
    public class FakeDiscountRepository : IDiscountRepository
    {
        public Discount Get(string code)
        {
            if (code == "87654321")
                return new Discount(10, DateTime.Now.AddDays(5));

            if (code == "99999999")
                return new Discount(10, DateTime.Now.AddDays(-5));

            return null;
        }
    }
}