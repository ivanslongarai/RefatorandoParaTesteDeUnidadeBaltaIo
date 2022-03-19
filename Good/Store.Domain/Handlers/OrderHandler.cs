using System.Linq;
using Flunt.Notifications;
using Store.Domain.Commands;
using Store.Domain.Commands.Interfaces;
using Store.Domain.Entities;
using Store.Domain.Handlers.Interfaces;
using Store.Domain.Repositories.Interfaces;
using Store.Domain.Utils;

namespace Store.Domain.Handlers
{
    public class OrderHandler :
        Notifiable,
        IHandler<CreateOrderCommand>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IDeliveryFeeRepository _deliveryFeeRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderHandler(
            ICustomerRepository customerRepository,
            IDeliveryFeeRepository deliveryFeeRepository,
            IDiscountRepository discountRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _deliveryFeeRepository = deliveryFeeRepository;
            _discountRepository = discountRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public ICommandResult Handle(CreateOrderCommand command)
        {
            // Fail Fast Validation
            command.Validate();

            if (command.Invalid)
            {
                AddNotification("Order", "Pedido inválido");
                return new GenericCommandResult(false, "Pedido inválido", null, command.Notifications);
            }

            // 1. Get Custumer
            var customer = _customerRepository.Get(command.Customer);
            if (customer == null)
            {
                AddNotification("Customer", "Cliente inválido");
                return new GenericCommandResult(false, "Cliente inválido", null, command.Notifications);
            }

            // 2. Get DeliveryFee Value
            var deliveryFee = _deliveryFeeRepository.Get(command.ZipCode);

            // 3. Get Discount
            var discount = _discountRepository.Get(command.PromoCode);

            // 4. Create Order
            var products = _productRepository.Get(ExtractGuids.Extract(command.Items)).ToList();
            var order = new Order(customer, deliveryFee, discount);

            if (command.Items.Count == 0)
            {
                AddNotification("Order", "O pedido não possui produtos");
                return new GenericCommandResult(false, "O pedido não possui produtos", null, command.Notifications);
            }

            foreach (var item in command.Items)
            {
                var product = products.Where(x => x.Id == item.Product).FirstOrDefault();
                order.AddItem(product, item.Quantity);
            }

            // 5. Group Notifications
            AddNotifications(order.Notifications);

            // 6. Verify Valid
            if (Invalid)
                return new GenericCommandResult(false, "Falha ao gerar o pedido", null, Notifications);

            // 7. Return
            _orderRepository.Save(order);
            return new GenericCommandResult(true, $"Pedido {order.Number} gerado com sucesso", order, null);
        }
    }
}