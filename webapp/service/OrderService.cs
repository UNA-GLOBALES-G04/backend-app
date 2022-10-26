using Microsoft.EntityFrameworkCore;
using webapp.data;
using webapp.model;

namespace webapp.service
{
    public class OrderService
    {
        private WebAppContext context;
        public OrderService(WebAppContext context)
        {
            this.context = context;
        }

        public Order? getOrder(string orderID)
        {
            return context.Orders.Find(orderID);
        }

        public IEnumerable<Order> getOrdersByUserProfileID(string userProfileID)
        {
            return context.Orders.Where(o => o.UserProfileId == userProfileID).ToList();
        }

        public IEnumerable<Order> getOrdersByServiceID(Guid serviceID, Order.OrderStatus[] statuses)
        {
            if (statuses.Length == 0)
            {
                return context.Orders.Where(o => o.ServiceId == serviceID).ToList();
            }
            return context.Orders.Where(o => o.ServiceId == serviceID && statuses.Contains(o.status)).ToList();
        }

        public bool existsOrder(Guid orderID)
        {
            return context.Orders.AsNoTracking().Any(o => o.Id == orderID);
        }

        public IEnumerable<Order> getAllOrders()
        {
            return context.Orders.ToList();
        }

        public Order? createOrder(Order order)
        {
            // the order will have a random GUID
            order.Id = Guid.NewGuid();
            // the direction cannot be empty
            if (order.direction == null || order.direction == "")
            {
                return null;
            }
            // the status will be pending
            order.status = Order.OrderStatus.PENDING;

            context.Orders.Add(order);
            if (context.SaveChanges() > 0)
            {
                return order;
            }
            return null;
        }

        public Order? updateOrder(Order order, bool isUser)
        {
            Order? orderToUpdate = context.Orders.Find(order.Id);
            if (orderToUpdate != null)
            {
                order.Id = orderToUpdate.Id;
                order.ServiceId = orderToUpdate.ServiceId;
                order.UserProfileId = orderToUpdate.UserProfileId;
                if (isUser)
                {
                    if (orderToUpdate.status == Order.OrderStatus.PENDING)
                    {
                        if (order.status == Order.OrderStatus.PENDING || order.status == Order.OrderStatus.REJECTED)
                        {
                            context.Orders.Update(order);
                            if (context.SaveChanges() > 0)
                            {
                                return order;
                            }
                            return null;
                        }
                    }
                }
                else
                {
                    order.requiredDate = orderToUpdate.requiredDate;
                    order.direction = orderToUpdate.direction;

                    // can only be updated if the status is pending or accepted
                    if (orderToUpdate.status == Order.OrderStatus.PENDING || orderToUpdate.status == Order.OrderStatus.ACCEPTED)
                    {
                        bool isDowngrade =
                            //
                            (order.status == Order.OrderStatus.ACCEPTED ||
                            order.status == Order.OrderStatus.REJECTED) &&
                            orderToUpdate.status == Order.OrderStatus.PENDING;
                        // cannot be a downgrade
                        if (!isDowngrade)
                        {
                            context.Orders.Update(order);
                            if (context.SaveChanges() > 0)
                            {
                                return order;
                            }
                            return null;
                        }
                    }
                }
            }
            return null;
        }

    }
}