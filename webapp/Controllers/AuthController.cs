using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapp.model.auth;
using webapp.service;
using webapp.util;

namespace webapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration configuration;
        private UserCredentialService userCredentialService;
        public AuthController(IConfiguration configuration, UserCredentialService userCredentialService)
        {
            this.configuration = configuration;
            this.userCredentialService = userCredentialService;
        }

        [HttpPost]
        public IActionResult login(UserCredential user)
        {
            // get the user from the database
            var userCredential = userCredentialService.getUserCredentialFromEmail(user.Email);
            if (userCredential != null)
            {
                var hash = Argon2.Hash(userCredential.Password);
                // verify the password
                if (Argon2.Verify(hash, userCredential.Password))
                {
                    // generate a JWT token
                    user.Id = userCredential.Id;
                    var stringToken = new TokenUtils(configuration).GenerateToken(user.Id);
                    return Ok(new { token = stringToken });
                }
            }

            return Unauthorized();
        }

        [HttpPost, Route("register")]
        public IActionResult register(UserCredential user)
        {
            // check if the user already exists
            user.Id = "";
            // chek if the user already exists
            if (userCredentialService.existsUserCredentialByEmail(user.Email))
            {
                return Unauthorized(new {
                    code = "ERR_USER_EXISTS",
                    message = $"User {user.Email} already exists"
                });
            }
            // create the user
            var userCredential = userCredentialService.createUserCredential(user);
            if (userCredential != null)
            {
                // generate a JWT token
                user.Id = userCredential.Id;
                var stringToken = new TokenUtils(configuration).GenerateToken(userCredential.Id);
                return Ok(new { token = stringToken });
            }
            return BadRequest(new
            {
                code = "unknown_error",
                message = "An unknown error occurred."
            });
        }

        [HttpPatch, Authorize]
        public IActionResult update(UserCredential user)
        {
            // check if the user already exists
            var userCredential = userCredentialService.getUserCredential(user.Id);
            if (userCredential != null)
            {
                userCredential = userCredentialService.updateUserCredential(userCredential);
                if (userCredential != null)
                {
                    // TODO: notify to redis that the user has been updated
                    // TODO: so it backlists all the user tokens
                    return Ok();
                }
            }
            return BadRequest("User could not be updated");
        }


    }
}
