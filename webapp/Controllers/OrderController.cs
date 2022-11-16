
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapp.model;
using webapp.service;

namespace webapp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IConfiguration configuration;
        private OrderService orderService;
        private ServiceService serviceService;
        private UserProfileService userProfileService;
        public OrderController(IConfiguration configuration, OrderService orderService, ServiceService serviceService, UserProfileService userProfileService)
        {
            this.configuration = configuration;
            this.orderService = orderService;
            this.serviceService = serviceService;
            this.userProfileService = userProfileService;
        }

        [HttpGet, Route("id/{orderID}"), Authorize]
        public IActionResult getOrder(string orderID)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            var order = orderService.getOrder(orderID);
            if (order != null)
            {
                return Ok(order);
            }

            return NotFound(
                    new
                    {
                        error_code = "order_not_found",
                        error_description = "The order was not found"
                    }
            );
        }

        [HttpGet, Route("MyOrders"), Authorize]
        public IActionResult getOrders()
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            var orders = orderService.getOrdersByUserProfileID(userID);
            if (orders != null)
            {
                return Ok(orders);
            }

            return NotFound(
                    new
                    {
                        error_code = "orders_not_found",
                        error_description = "The orders were not found"
                    }
            );
        }

        [HttpGet, Route("MyOrders/active"), Authorize]
        public IActionResult getActiveOrders()
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            var orders = orderService.getActiveUserOrders(userID);
            if (orders != null)
            {
                return Ok(orders);
            }

            return NotFound(
                    new
                    {
                        error_code = "orders_not_found",
                        error_description = "The orders were not found"
                    }
            );
        }

        // get the orders by serviceID and a array of status
        [HttpGet, Route("{serviceID}/orders"), Authorize]
        public IActionResult getServiceOrders(Guid serviceID, [FromQuery] Order.OrderStatus[] status)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            // get the service
            var service = serviceService.getService(serviceID.ToString(), true);
            if (service == null)
            {
                return NotFound(
                    new
                    {
                        error_code = "service_not_found",
                        error_description = "The service was not found"
                    }
                );
            }
            else
            {
                // check if the user is the owner of the service
                if (service.UserProfileId != userID)
                {
                    return Unauthorized(
                        new
                        {
                            error_code = "ERR_USER_NOT_OWNER",
                            error_description = "You are not authorized to access this resource"
                        }
                    );
                }
            }
            var orders = orderService.getOrdersByServiceID(serviceID, status);
            if (orders != null)
            {
                return Ok(orders);
            }

            return NotFound(
                    new
                    {
                        error_code = "orders_by_service_not_found",
                        error_description = "The orders of service were not found"
                    }
            );
        }

        // get the orders by vendorID and a array of status
        [HttpGet, Route("vendor/MyOrders"), Authorize]
        public IActionResult getVendorOrders([FromQuery] Order.OrderStatus[] status)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            // get the user profile
            var userProfile = userProfileService.getUserProfile(userID);
            if (userProfile == null)
            {
                return NotFound(
                    new
                    {
                        error_code = "user_profile_not_found",
                        error_description = "The user profile was not found"
                    }
                );
            }
            var orders = orderService.getOrdersByVendorID(userID, status);
            if (orders != null)
            {
                return Ok(orders);
            }

            return NotFound(
                    new
                    {
                        error_code = "orders_by_vendor_not_found",
                        error_description = "The orders of vendor were not found"
                    }
            );
        }


        [HttpPost, Authorize]
        public IActionResult addOrder(Order order)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            // check if the order has the same user id
            if (order.UserProfileId == userID)
            {
                if (!userProfileService.existsUserProfile(userID))
                {
                    return NotFound(
                        new
                        {
                            error_code = "user_profile_not_found",
                            error_description = "The user profile was not found"
                        }
                    );
                }
                // add the new order
                order.rating = null;
                Order? result = orderService.createOrder(order);
                if (result != null)
                {
                    return Ok(result);
                }
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error_code = "order_not_created",
                        error_description = "The order was not created, try again"
                    }
                );
            }
            return Unauthorized(new { error_code = "ERR_NON_MATCHING_USER_ID" });
        }

        [HttpPost, Route("update") , Authorize]
        public IActionResult updateOrder(Order order)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            // check if the order has the same user id
            if (order.UserProfileId == userID)
            {
                // update the order
                Order? result = orderService.updateOrder(order, true);
                if (result != null)
                {
                    return Ok(result);
                }
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error_code = "order_not_updated",
                        error_description = "The order was not updated, try again"
                    }
                );
            }
            return Unauthorized(new { error_code = "ERR_NON_MATCHING_USER_ID" });
        }

        [HttpPost, Route("{serviceID}/{orderID}/accept"), Authorize]
        public IActionResult acceptOrder(Guid serviceID, Guid orderID)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            // get the service
            var service = serviceService.getService(serviceID.ToString(), true);
            if (service == null)
            {
                return NotFound(
                    new
                    {
                        error_code = "service_not_found",
                        error_description = "The service was not found"
                    }
                );
            }
            else
            {
                // check if the user is the owner of the service
                if (service.UserProfileId != userID)
                {
                    return Unauthorized(
                        new
                        {
                            error_code = "ERR_USER_NOT_OWNER",
                            error_description = "You are not authorized to access this resource"
                        }
                    );
                }
            }
            // get the order
            var order = orderService.getOrder(orderID.ToString());
            if (order == null)
            {
                return NotFound(
                    new
                    {
                        error_code = "order_not_found",
                        error_description = "The order was not found"
                    }
                );
            }
            order.current_status = Order.OrderStatus.ACCEPTED;
            // update the order
            Order? result = orderService.updateOrder(order, false);
            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error_code = "order_not_updated",
                    error_description = "The order was not updated, try again"
                }
            );
        }

        [HttpPost, Route("{serviceID}/{orderID}/complete"), Authorize]
        public IActionResult completeOrder(Guid serviceID, Guid orderID)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            // get the service
            var service = serviceService.getService(serviceID.ToString(), true);
            if (service == null)
            {
                return NotFound(
                    new
                    {
                        error_code = "service_not_found",
                        error_description = "The service was not found"
                    }
                );
            }
            else
            {
                // check if the user is the owner of the service
                if (service.UserProfileId != userID)
                {
                    return Unauthorized(
                        new
                        {
                            error_code = "ERR_USER_NOT_OWNER",
                            error_description = "You are not authorized to access this resource"
                        }
                    );
                }
            }
            // get the order
            var order = orderService.getOrder(orderID.ToString());
            if (order == null)
            {
                return NotFound(
                    new
                    {
                        error_code = "order_not_found",
                        error_description = "The order was not found"
                    }
                );
            }
            // copy the order
            var newOrder = new Order(
                order.Id,
                order.ServiceId,
                order.UserProfileId,
                order.requiredDate,
                order.direction,
                Order.OrderStatus.COMPLETED,
                order.rating,
                order.description
            );
            // update the order
            Order? result = orderService.updateOrder(newOrder, false);
            if (result != null)
            {
                return Ok(result);
            }
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    error_code = "order_not_updated",
                    error_description = "The order was not updated, try again"
                }
            );
        }

        [HttpDelete, Route("id/{orderID}/cancel"), Authorize]
        public IActionResult cancelOrder(string orderID)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            // get the order
            var order = orderService.getOrder(orderID);
            if (order != null)
            {
                // check if the order has the same user id
                if (order.UserProfileId == userID)
                {
                    // cancel the order
                    // cast the id to Guid
                    if (Guid.TryParse(orderID, out Guid orderGuid))
                    {
                        Order? result = orderService.deleteOrder(orderGuid, true);
                        if (result != null)
                        {
                            return Ok(result);
                        }
                        return StatusCode(
                            StatusCodes.Status500InternalServerError,
                            new
                            {
                                error_code = "order_not_cancelled",
                                error_description = "The order was not cancelled, try again"
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(
                            StatusCodes.Status400BadRequest,
                            new
                            {
                                error_code = "invalid_order_id",
                                error_description = "The order id is invalid"
                            }
                        );
                    }

                }
                return Unauthorized(new { error_code = "ERR_NON_MATCHING_USER_ID" });
            }
            return NotFound(
                    new
                    {
                        error_code = "order_not_found",
                        error_description = "The order was not found"
                    }
            );
        }

        [HttpDelete, Route("id/{orderID}/reject"), Authorize]
        public IActionResult rejectOrder(string orderID)
        {
            var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userID = (subClaim != null) ? subClaim.Value : "";
            if (userID == "")
            {
                return Unauthorized(
                    new
                    {
                        error_code = "invalid_token",
                        error_description = "The token is invalid, please login again"
                    }
                );
            }
            // get the order
            var order = orderService.getOrder(orderID);
            if (order != null)
            {

                // check if the service of the order has the same user id
                var service = serviceService.getService(order.ServiceId.ToString(), false);
                if (service != null)
                {
                    if (service.UserProfileId == userID)
                    {
                        // reject the order
                        // cast the id to Guid
                        if (Guid.TryParse(orderID, out Guid orderGuid))
                        {
                            Order? result = orderService.deleteOrder(orderGuid, false);
                            if (result != null)
                            {
                                return Ok(result);
                            }
                            return StatusCode(
                                StatusCodes.Status500InternalServerError,
                                new
                                {
                                    error_code = "order_not_rejected",
                                    error_description = "The order was not rejected, try again"
                                }
                            );
                        }
                        else
                        {
                            return StatusCode(
                                StatusCodes.Status400BadRequest,
                                new
                                {
                                    error_code = "invalid_order_id",
                                    error_description = "The order id is invalid"
                                }
                            );
                        }
                    }
                    return Unauthorized(new { error_code = "ERR_NON_MATCHING_USER_ID" });
                }
                return NotFound(
                    new
                    {
                        error_code = "service_not_found",
                        error_description = "The service was not found"
                    }
                );
            }
            return NotFound(
                    new
                    {
                        error_code = "order_not_found",
                        error_description = "The order was not found"
                    }
            );
        }
    }
};