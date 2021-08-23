using Jwt.Server.Data;
using Jwt.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Jwt.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {

        private readonly ILogger<TokenController> _logger;
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenController(ILogger<TokenController> logger, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _userManager = userManager;
        }



        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            var response = await CheckUnPw(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }


        //// Jwt.Server also needs Auth - because the Users DB lives here in Jwt.Server, we need to provide an internal Api for user management (). This Auth is setup in Startup.cs
        //
        //[Authorize (UserType="worker")]
        //[HttpGet("users")]
        //public IEnumerable<ApplicationUser> Users()
        //{
        //    var users = _userManager.Users.ToList();
        //    return users;
        //}



        // HELPERS
        public async Task<AuthenticateResponse> CheckUnPw(AuthenticateRequest model)
        {
            var user = _userManager.Users.FirstOrDefault(o => o.Email == model.Username);

            if (user == null) 
                return null;

            if (_userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
            {
                // password is incorrect
                return null;
            }


            // authentication successful so generate jwt token
            var claims = await _userManager.GetClaimsAsync(user);
            var token = GenerateJwtToken(user, claims);

            return new AuthenticateResponse(user, token);
        }

        private string GenerateJwtToken(ApplicationUser user, IList<Claim> claims)
        {
            // generate token that is valid for 10 years (to keep the question working on StackOverflow for a while)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            // Add the users claims to the Jwt
            var clms = new ClaimsIdentity(new[] { new Claim("id", user.Id) });
            foreach (var claim in claims)
            {
                clms.AddClaim(new Claim(claim.Type, claim.Value));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = clms, // Assigning the claims to 'Subject' has same outcome as adding to 'Claims' - both do the same job (except subject cannot be empty). 
                //Claims = clms, 
                Expires = DateTime.UtcNow.AddYears(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
