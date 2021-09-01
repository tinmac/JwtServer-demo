using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jwt.Blazor
{

    public class MyAuthenticationStateProvider : AuthenticationStateProvider
    {
        string UserId;
        string Password;

        public void LoadUser(string _UserId, string _Password)
        {
            UserId = _UserId;
            Password = _Password;
        }


        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // below is from: https://docs.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-5.0#authenticationstateprovider-service-1

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "billy@email.com"),
                new Claim("agency", "true"),

            }, "Fake authentication type");

            var user = new ClaimsPrincipal(identity);

            return Task.FromResult( new AuthenticationState(user));
        }

    }
}
