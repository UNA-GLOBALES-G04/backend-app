
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
        private UserProfileService userProfileService;
        public OrderController(IConfiguration configuration, OrderService orderService, UserProfileService userProfileService)
        {
            this.configuration = configuration;
            this.orderService = orderService;
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
        // [HttpGet, Route("id/{orderID}"), Authorize]
        // public IActionResult getOrderByServiceID(string serviceID)
        // {
        //     var subClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //     string userID = (subClaim != null) ? subClaim.Value : "";
        //     if (userID == "")
        //     {
        //         return Unauthorized(
        //             new
        //             {
        //                 error_code = "invalid_token",
        //                 error_description = "The token is invalid, please login again"
        //             }
        //         );
        //     }
        //     var orders = orderService.getOrdersByorderID(serviceID);
        //     if (orders != null)
        //     {
        //         return Ok(orders);
        //     }

        //     return NotFound(
        //             new
        //             {
        //                 error_code = "orders_not_found",
        //                 error_description = "The order was not found"
        //             }
        //     );
        // }

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

        [HttpPut, Authorize]
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
    }
};