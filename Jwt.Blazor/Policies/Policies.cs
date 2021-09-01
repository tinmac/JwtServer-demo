using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt.Blazor
{
    public static class MyPolicies
    {
        public const string HasSub= "HasSub";

        public static AuthorizationPolicy HasSubPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim("paid_up", "true")
                                                   .Build();
        }


        public const string IsMultiUser = "IsMultiUser";

        public static AuthorizationPolicy IsMultiUserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim("multi_user", "true")
                                                   .Build();
        }


        public const string IsWorker = "IsWorker";

        public static AuthorizationPolicy IsWorkerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim("worker", "true")
                                                  // .RequireClaim("paid_up", "true")
                                                   .Build();
        }

        public const string IsAgent = "IsAgent";

        public static AuthorizationPolicy IsAgentPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim("agent", "true")
                                                   //.RequireClaim("paid_up", "true")
                                                   .Build();
        }


        public const string IsBooker = "IsBooker";

        public static AuthorizationPolicy IsBookerPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .RequireClaim("booker", "true")
                                                  // .RequireClaim("paid_up", "true")
                                                   .Build();
        }

    }
}
