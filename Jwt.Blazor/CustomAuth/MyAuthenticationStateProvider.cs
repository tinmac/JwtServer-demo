using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Jwt.Blazor
{

    public class MyAuthenticationStateProvider : AuthenticationStateProvider
    {
        IEnumerable<Claim> _claims;

        public void LoadUser(IEnumerable<Claim> claims)
        {
            _claims = claims;
        }


        // What causes this to run?
        // I want to run it myself
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // below is from: https://code-maze.com/authenticationstateprovider-blazor-webassembly/
            Debug.WriteLine("");
            Debug.WriteLine($"GetAuthenticationStateAsync  fired...");

            // await Task.Delay(1500);

            //bool IsAuthed = true;
           // IsAuthed = false;
            if (_claims != null && _claims.Count() > 0)
            {

                //var identity = new ClaimsIdentity(new[]
                //{
                //    new Claim(ClaimTypes.Name, "billy@email.com"),
                //    new Claim("agentt", "true"),
                //}, "Fake authentication type");


                var identity = new ClaimsIdentity(_claims);


                var user = new ClaimsPrincipal(identity);
                Debug.WriteLine($"    Authorized {user.Identity.Name}");

                foreach(var claim in _claims)
                {
                    Debug.WriteLine($"    {claim.Type} : {claim.Value}");
                }

                return new AuthenticationState(user);
            }
            else
            {
                var unauthorizedUser = new ClaimsIdentity();// Unauthorized
                                                            //var unauthorizedUser = new ClaimsIdentity(null, "my_auth");// Authorized, This will authenticate because we passed the 'authenticationType'
                Debug.WriteLine($"    Unauthorized");

                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(unauthorizedUser)));
            }


        }
    }
}
