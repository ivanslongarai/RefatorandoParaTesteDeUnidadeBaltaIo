using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Store.Domain.Entities;
using Store.Domain.Enums;

namespace Store.Tests.Domain

// Read, Green, Refactory

{
    [TestClass]
    public class OrderTests
    {
        private readonly Customer _customer = new Customer("Ivan Longarai", "eu@gmail.com");
        private readonly Product _product = new Product("Produto 1", 10, true);
        private readonly Discount _discount = new Discount(10, DateTime.Now.AddDays(5));

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnOrderShoudReturnANumberWith8Caracteres()
        {
            var order = new Order(_customer);
            Assert.AreEqual(8, order.Number.Length);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnOrderShoudReturnAStatusWaitingPayment()
        {
            var order = new Order(_customer);
            Assert.AreEqual(order.Status, EOrderStatus.WaitingPayment);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAPaymentShoudReturnAStatusWaitingDelivery()
        {
            var order = new Order(_customer);
            order.AddItem(_product);
            order.Pay(_product.Price);
            Assert.AreEqual(order.Status, EOrderStatus.WaitingDelivery);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenACanceledOrderShoudReturnAStatusCanceled()
        {
            var order = new Order(_customer);
            order.Cancel();
            Assert.AreEqual(order.Status, EOrderStatus.Canceled);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnItemWithoutAProductShoudReturnNoAddedProduct()
        {
            var order = new Order(_customer);
            order.AddItem(null, 10);
            Assert.AreEqual(order.Items.Count, 0);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnItemWithZeroQuantityOrLessShouldReturnNoAddedItem()
        {
            var order = new Order(_customer);
            order.AddItem(_product, 0);
            Assert.AreEqual(order.Items.Count, 0);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenANewOrderShouldReturnTotalOf50()
        {
            var order = new Order(_customer, 10, _discount);
            order.AddItem(_product, 5);
            Assert.AreEqual(order.Total(), 50);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnExpiredDiscountShoudReturnAOrderWithTotal60()
        {
            var expiredDiscount = new Discount(10, DateTime.Now.AddDays(-1));
            var order = new Order(_customer, 10, expiredDiscount);
            order.AddItem(_product, 5);
            Assert.AreEqual(order.Total(), 60);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnInvalidDescontShoudReturnAOrderWithTotal60()
        {
            var order = new Order(_customer, 10, null);
            order.AddItem(_product, 5);
            Assert.AreEqual(order.Total(), 60);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnValid10DescontShoudReturnAOrderWithTotal50()
        {
            var order = new Order(_customer, 10, _discount);
            order.AddItem(_product, 5);
            Assert.AreEqual(order.Total(), 50);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenA10DeliveryFeeShoudReturnAnOrderTotal60()
        {
            var order = new Order(_customer, 10, _discount);
            order.AddItem(_product, 6);
            Assert.AreEqual(order.Total(), 60);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnOrderWithoutACustumerShoulReturnAnInvalidOrder()
        {
            var order = new Order(null, 10, _discount);
            Assert.AreEqual(order.Valid, false);
        }

        [TestMethod]
        [TestCategory("Domain")]
        public void GivenAnOrderShoulReturnStatusConcluded()
        {
            var order = new Order(_customer, 10, _discount);
            order.AddItem(_product, 1);
            order.Pay(10);
            order.Conclude();
            Assert.AreEqual(order.Status, EOrderStatus.Concluded);
        }
    }
}
