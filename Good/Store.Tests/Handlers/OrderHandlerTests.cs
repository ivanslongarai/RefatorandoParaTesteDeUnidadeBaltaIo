using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Store.Domain.Commands;
using Store.Domain.Handlers;
using Store.Domain.Repositories.Interfaces;
using Store.Tests.Repositories;

namespace Store.Tests.Handlers
{
    [TestClass]
    public class OrderHandlerTests
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderHandlerTests()
        {
            _customerRepository = new FakeCustomerRepository();
            _deliveryFeeRepository = new FakeDeliveryFeeRepository();
            _discountRepository = new FakeDiscountRepository();
            _orderRepository = new FakeOrderRepository();
            _productRepository = new FakeProductRepository();
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void GivenANonExistentCustomerShoulReturnANonCreatedOrder()
        {
            var command = new CreateOrderCommand();
            command.Customer = "12345678912";
            command.ZipCode = "13411080";
            command.PromoCode = "12345678";
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));

            var handler = GetHandler();
            handler.Handle(command);

            Assert.AreEqual(handler.Valid, false);
        }

        public void GivenAnExistentCustomerShoulReturnACreatedOrder()
        {
            var command = new CreateOrderCommand();
            command.Customer = "12345678911";
            command.ZipCode = "13411080";
            command.PromoCode = "12345678";
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));

            var handler = GetHandler();
            handler.Handle(command);

            Assert.AreEqual(handler.Valid, true);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void GivenAnInvalidCepShoulReturnANonCreatedOrder()
        {
            var command = new CreateOrderCommand();
            command.Customer = "12345678911";
            command.ZipCode = "";
            command.PromoCode = "12345678";
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
            command.Validate();
            Assert.AreEqual(command.Valid, false);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void GivenAnInvalidPromoCodeShoulReturnACreatedOrder()
        {
            var command = new CreateOrderCommand();
            command.Customer = "12345678911";
            command.ZipCode = "13411080";
            command.PromoCode = "invalid_promocode";
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
            var handler = GetHandler();
            handler.Handle(command);
            Assert.AreEqual(handler.Valid, true);
        }

        [TestMethod]
        [TestCategory("Handlers")]

        public void GivenAnOrderWithoutItemsShouldReturnANonCreatedOrder()
        {
            var command = new CreateOrderCommand();
            command.Customer = "12345678911";
            command.ZipCode = "13411080";
            command.PromoCode = "invalid_promocode";
            var handler = GetHandler();
            handler.Handle(command);
            Assert.AreEqual(handler.Valid, false);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void GivenAnInvalidCommandShouldReturnANonCreatedOrder()
        {
            var command = new CreateOrderCommand();
            var handler = GetHandler();
            handler.Handle(command);
            Assert.AreEqual(handler.Valid, false);
        }

        [TestMethod]
        [TestCategory("Handlers")]
        public void GivenAValidCommandShouldReturnACreatedOrder()
        {
            var command = new CreateOrderCommand();
            command.Customer = "12345678911";
            command.ZipCode = "13411080";
            command.PromoCode = "12345678";
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));
            command.Items.Add(new CreateOrderItemCommand(Guid.NewGuid(), 1));

            var handler = GetHandler();
            handler.Handle(command);

            Assert.AreEqual(handler.Valid, true);
        }

        private OrderHandler GetHandler()
        {
            return new OrderHandler(_customerRepository, _deliveryFeeRepository, _discountRepository, _productRepository, _orderRepository);
        }
    }
}