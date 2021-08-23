using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt.Blazor.Policies
{
    public static class MyPolicies
    {

        public const string IsWorker = "IsWorker";

        public static AuthorizationPolicy IsWorkerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim("worker", "true")
                                                   .Build();
        }


        public const string IsBooker = "IsBooker";

        public static AuthorizationPolicy IsBookerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim("booker", "true")
                                                   .Build();
        }

    }
}
