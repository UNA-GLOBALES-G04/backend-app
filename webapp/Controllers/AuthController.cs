using Microsoft.AspNetCore.Mvc;
using webapp.model;
using webapp.util;

namespace webapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration configuration;
        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost, Route("login")]
        public IActionResult login(LoginDTO user)
        {
            // at the moment we will not validate the user
            // user.UserName == "user" 
            if (user.Password == "password")
            {
                var stringToken = new TokenUtils(configuration).GenerateToken(user.UserName);
                return Ok(new { token = stringToken });
            }
            return Unauthorized();
        }
    }
}
