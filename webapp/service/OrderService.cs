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
            if (Guid.TryParse(orderID, out Guid orderGuid))
            {
                return context.Orders.Find(orderGuid);
            }
            return null;
        }

        public IEnumerable<Order> getOrdersByUserProfileID(string userProfileID)
        {
            return context.Orders.Where(o => o.UserProfileId == userProfileID).ToList();
        }

        public IEnumerable<Order> getActiveUserOrders(string userProfileID)
        {
            return context.Orders
                .Where(o => o.UserProfileId == userProfileID &&
                        o.current_status != Order.OrderStatus.CANCELLED &&
                        o.current_status != Order.OrderStatus.COMPLETED &&
                        o.current_status != Order.OrderStatus.REJECTED).ToList();
        }

        public IEnumerable<Order> getOrdersByServiceID(Guid serviceID, Order.OrderStatus[] statuses)
        {
            if (statuses.Length == 0)
            {
                return context.Orders.Where(o => o.ServiceId == serviceID).ToList();
            }
            return context.Orders.Where(o => o.ServiceId == serviceID && statuses.Contains(o.current_status)).ToList();
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
            order.current_status = Order.OrderStatus.PENDING;

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
                    if (orderToUpdate.current_status == Order.OrderStatus.PENDING)
                    {
                        if (order.current_status == Order.OrderStatus.PENDING || order.current_status == Order.OrderStatus.REJECTED)
                        {
                            if (orderToUpdate != null)
                            {
                                // the rating cannot be updated by the user
                                // unless the order is completed
                                order.rating = orderToUpdate.rating;

                                // update orderToUpdate
                                context.Entry(orderToUpdate).CurrentValues.SetValues(order);
                                if (context.SaveChanges() > 0)
                                {
                                    return orderToUpdate;
                                }
                            }
                            return null;
                        }
                    }
                    // check if the order is completed
                    // so the user can update the rating
                    if (orderToUpdate.current_status == Order.OrderStatus.COMPLETED)
                    {
                        // the user can only update the rating
                        order.requiredDate = orderToUpdate.requiredDate;
                        order.direction = orderToUpdate.direction;
                        order.current_status = orderToUpdate.current_status;
                        order.description = orderToUpdate.description;

                        context.Entry(orderToUpdate).CurrentValues.SetValues(order);
                        if (context.SaveChanges() > 0)
                        {
                            return orderToUpdate;
                        }
                    }
                }
                else
                {
                    order.requiredDate = orderToUpdate.requiredDate;
                    order.direction = orderToUpdate.direction;
                    order.rating = orderToUpdate.rating;
                    order.description = orderToUpdate.description;

                    // can only be updated if the status is pending or accepted
                    if (orderToUpdate.current_status == Order.OrderStatus.PENDING || orderToUpdate.current_status == Order.OrderStatus.ACCEPTED)
                    {
                        bool isDowngrade =
                            //
                            (order.current_status == Order.OrderStatus.ACCEPTED ||
                            order.current_status == Order.OrderStatus.REJECTED) &&
                            orderToUpdate.current_status == Order.OrderStatus.PENDING;
                        // cannot be a downgrade
                        if (!isDowngrade)
                        {
                            // check if the order is the same
                            if (orderToUpdate.current_status != order.current_status)
                            {
                                // update orderToUpdate
                                context.Entry(orderToUpdate).CurrentValues.SetValues(order);
                                if (context.SaveChanges() > 0)
                                {
                                    return orderToUpdate;
                                }
                            }
                            else
                            {
                                return orderToUpdate;
                            }
                        }
                    }
                }
            }
            return null;
        }


        public Order? deleteOrder(Guid orderID, bool isUser)
        {
            Order? orderToDelete = context.Orders.Find(orderID);
            if (orderToDelete != null)
            {
                // set the status to cancelled if is user else rejected
                if (isUser)
                {
                    // can only be cancelled if the status is pending
                    if (orderToDelete.current_status == Order.OrderStatus.PENDING)
                    {
                        orderToDelete.current_status = Order.OrderStatus.CANCELLED;
                        context.Entry(orderToDelete).CurrentValues.SetValues(orderToDelete);
                        if (context.SaveChanges() > 0)
                        {
                            return orderToDelete;
                        }
                    }
                }
                else
                {
                    // can only be rejected if the status is pending or accepted
                    if (orderToDelete.current_status == Order.OrderStatus.PENDING || orderToDelete.current_status == Order.OrderStatus.ACCEPTED)
                    {
                        orderToDelete.current_status = Order.OrderStatus.REJECTED;
                        context.Entry(orderToDelete).CurrentValues.SetValues(orderToDelete);
                        if (context.SaveChanges() > 0)
                        {
                            return orderToDelete;
                        }
                    }
                }
            }
            return null;
        }
    }
}