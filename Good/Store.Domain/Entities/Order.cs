using System;
using System.Collections.Generic;
using System.Linq;
using Flunt.Validations;
using Store.Domain.Enums;

namespace Store.Domain.Entities
{
    public class Order : Entity
    {
        private readonly IList<OrderItem> _items;

        public Order(Customer customer, decimal deliveryFee = 0, Discount discount = null)
        {
            AddNotifications(
                new Contract()
                    .Requires()
                    .IsNotNull(customer, "Customer", "Cliente inválido")
            );

            Customer = customer;
            Date = DateTime.Now;
            Number = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
            Status = EOrderStatus.WaitingPayment;
            DeliveryFee = deliveryFee;
            Discount = discount;
            _items = new List<OrderItem>();
        }

        public Customer Customer { get; private set; }
        public DateTime Date { get; private set; }
        public string Number { get; private set; }
        public IReadOnlyCollection<OrderItem> Items { get => _items.ToArray(); }
        public decimal DeliveryFee { get; private set; }
        public Discount Discount { get; private set; }
        public EOrderStatus Status { get; private set; }

        public void AddItem(Product product, int quantity = 1)
        {
            var item = new OrderItem(product, quantity);
            if (item.Valid)
                _items.Add(item);
        }

        public decimal Total()
        {
            decimal total = 0;
            foreach (var item in Items)
            {
                total += item.Total();
            }

            total += DeliveryFee;
            total -= Discount != null ? Discount.Value() : 0;

            return total;
        }

        public void Pay(decimal amount)
        {
            if (amount == Total())
                this.Status = EOrderStatus.WaitingDelivery;
        }

        public void Cancel()
        {
            Status = EOrderStatus.Canceled;
        }

        public void Conclude()
        {
            if (this.Status == EOrderStatus.WaitingDelivery)
                this.Status = EOrderStatus.Concluded;
        }
    }
}