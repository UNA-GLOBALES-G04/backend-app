
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using webapp.util;

namespace webapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppAuthController : ControllerBase
    {
        private IConfiguration configuration;
        public AppAuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost, Route("login")]
#if DEBUG
        [ApiExplorerSettings(IgnoreApi = false)]
#else
        [ApiExplorerSettings(IgnoreApi = true)]
#endif  
        public IActionResult login(String jwtToken)
        {
            var token = ValidateToken(jwtToken);
            if (ValidateNonce(token))
            {
                // get the user-id from the token (sub)
                if (token.Claims.ContainsKey("sub"))
                {
                    string userId = token.Claims["sub"].ToString() ?? "";
                    var newToken = new TokenUtils(configuration).GenerateToken(userId);
                    // return the new token
                    return Ok(new { token = newToken });
                }
                return Unauthorized();
            }

            return Unauthorized();
        }

        // validate the token
        private TokenValidationResult ValidateToken(string token)
        {
            var keyPath = configuration["AppJwt:PublicKeyPath"];
            var keyData = System.IO.File.ReadAllText(keyPath);
            var key = ECDsa.Create();
            key.ImportFromPem(keyData);
            var result = new JsonWebTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = configuration["AppJwt:Issuer"],
                ValidAudience = configuration["AppJwt:Audience"],
                // ECDSA public key
                IssuerSigningKey = new ECDsaSecurityKey(key),
            });
            return result;
        }

        private bool ValidateNonce(TokenValidationResult token)
        {
            // check if the token contains the nonce
            if (token.Claims.ContainsKey("nonce"))
            {
                // get the nonce from the token
                var nonce = token.Claims["nonce"];
                // TODO: check if the nonce is valid
                return true;
            }
            return false;
        }


    }
}
