using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapp.model;
using webapp.service;

namespace webapp.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private IConfiguration configuration;
        private UserProfileService userProfileService;
        public UserProfileController(IConfiguration configuration, UserProfileService userProfileService)
        {
            this.configuration = configuration;
            this.userProfileService = userProfileService;
        }

        [HttpGet, Route("profile/user"), Authorize]
        public IActionResult getUserProfile()
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
            var userProfile = userProfileService.getUserProfile(userID);
            if (userProfile != null)
            {
                return Ok(userProfile);
            }

            return NotFound(
                    new
                    {
                        error_code = "user_profile_not_found",
                        error_description = "The user profile data was not found, try creating it"
                    }
            );
        }
        [HttpPost, Route("profile/user"), Authorize]
        public IActionResult createUserProfile([FromBody] UserProfile userProfile)
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
            if (userProfileService.existsUserProfile(userID))
            {
                return Conflict(
                    new
                    {
                        error_code = "user_profile_exists",
                        error_description = "The user profile data already exists, try updating it"
                    }
                );
            }

            userProfile.Id = userID;
            var createdUserProfile = userProfileService.createUserProfile(userProfile);
            if (createdUserProfile != null)
            {
                return Ok(createdUserProfile);
            }

            return BadRequest(
                    new
                    {
                        error_code = "err_post_user_profile",
                        error_description = "There was an error creating the user profile"
                    }
            );
        }

        // update user profile
        [HttpPost, Route("profile/user/update"), Authorize]
        public IActionResult updateUserProfile([FromBody] UserProfile userProfile)
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
            if (!userProfileService.existsUserProfile(userID))
            {
                return NotFound(
                    new
                    {
                        error_code = "user_profile_not_found",
                        error_description = "The user profile data was not found, try creating it"
                    }
                );
            }

            userProfile.Id = userID;
            var updatedUserProfile = userProfileService.updateUserProfile(userProfile, true);
            if (updatedUserProfile != null)
            {
                return Ok(updatedUserProfile);
            }

            return BadRequest(
                    new
                    {
                        error_code = "err_put_user_profile",
                        error_description = "There was an error updating the user profile"
                    }
            );
        }
    }
}