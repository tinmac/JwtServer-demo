
using Jwt.Server.Data;

namespace Jwt.Server.Models
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(ApplicationUser user, string token)
        {
            Id = user.Id;
            Token = token;
        }
    }
}