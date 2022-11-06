
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapp.model;
using webapp.model.filter;
using webapp.service;

namespace webapp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private IConfiguration configuration;
        private ServiceService serviceService;
        private UserProfileService userProfileService;
        public ServiceController(IConfiguration configuration, ServiceService serviceService, UserProfileService userProfileService)
        {
            this.configuration = configuration;
            this.serviceService = serviceService;
            this.userProfileService = userProfileService;
        }

        [HttpGet, Route(""), Authorize]
        public IActionResult GetAllServices()
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

            var services = serviceService.getAllServices();
            return Ok(services);
        }

        [HttpPost, Route("search"), Authorize]
        public IActionResult GetServicesByFilter(ServiceFilterDTO filter)
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

            var services = serviceService.getServicesByFilter(filter.name, filter.tags);
            return Ok(services);
        }

        [HttpGet, Route("id/{serviceID}"), Authorize]
        public IActionResult getService(string serviceID)
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
            var service = serviceService.getService(serviceID, true);
            if (service != null)
            {
                return Ok(service);
            }

            return NotFound(
                    new
                    {
                        error_code = "service_not_found",
                        error_description = "The service was not found"
                    }
            );
        }

        [HttpGet, Route("MyServices"), Authorize]
        public IActionResult getServices()
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
            var services = serviceService.getServicesByUserProfileID(userID, true);
            if (services != null)
            {
                return Ok(services);
            }

            return NotFound(
                    new
                    {
                        error_code = "services_not_found",
                        error_description = "The services were not found"
                    }
            );
        }

        [HttpPost, Authorize]
        public IActionResult addService(Service service)
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
            // check if the service has the same user id
            if (service.UserProfileId == userID)
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
                // add the new service
                Service? result = serviceService.createService(service);
                if (result != null)
                {
                    return Ok(result);
                }
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error_code = "service_not_created",
                        error_description = "The service was not created, try again"
                    }
                );
            }
            return Unauthorized(new { error_code = "ERR_NON_MATCHING_USER_ID" });
        }

        [HttpPut, Authorize]
        public IActionResult updateService(Service service)
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
            // check if the service has the same user id
            if (service.UserProfileId == userID)
            {
                // update the service
                Service? result = serviceService.updateService(service, true);
                if (result != null)
                {
                    return Ok(result);
                }
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error_code = "service_not_updated",
                        error_description = "The service was not updated, try again"
                    }
                );
            }
            return Unauthorized(new { error_code = "ERR_NON_MATCHING_USER_ID" });
        }
    }
};