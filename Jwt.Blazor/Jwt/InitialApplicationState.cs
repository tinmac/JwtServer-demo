using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt.Blazor.Auth
{
    // AccessToken solution From : https://docs.microsoft.com/en-us/aspnet/core/blazor/security/server/additional-scenarios?view=aspnetcore-5.0

    public class InitialApplicationState
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
