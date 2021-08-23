using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Jwt.Blazor.Auth
{
    public class JwtAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtAuthorizationMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            // Of the Request/Response cycle - this is the 'Request' & it should contain a JWT in the 'Authorization' header.
            // If there isnt a token there then the user will be 'Unauthorized'
            //
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                attachUserToContext(context, token);

            await _next(context);
        }

        private void attachUserToContext(HttpContext context,  string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                #region Logic

                // I want to rely soley on claims and not touch the Users DB (which would have to be done via an api as the DB is in another project)
                // var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                // attach user to context on successful jwt validation
                // var usr = userService.GetById(userId);
                // context.Items["User"] = usr;

                // Here I set a 'context.Item' with a key of 'user_type' and set is value to the claim that is true.
                // The 'context.item["user_type"]' is read and used by the 'AuthorizeAttribute'  
                #endregion


                var WorkerClaim = jwtToken.Claims?.FirstOrDefault(o => o.Type.ToLower() == "worker");
                var AgentClaim = jwtToken.Claims?.FirstOrDefault(o => o.Type.ToLower() == "agent");
                var BookerClaim = jwtToken.Claims?.FirstOrDefault(o => o.Type.ToLower() == "booker");

                string UserType = "";

                if (WorkerClaim != null && WorkerClaim.Value == "true")
                {
                    context.Items["user_type"] = "worker";
                    UserType = "Worker";
                }
                if (AgentClaim != null && AgentClaim.Value == "true")
                {
                    context.Items["user_type"] = "agent";
                    UserType = "Agent";
                }
                if (BookerClaim != null && BookerClaim.Value == "true")
                {
                    context.Items["user_type"] = "booker";
                    UserType = "Booker";
                }



                Debug.WriteLine($"Jwt Middleware   ==>   user_type: {UserType}");
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}