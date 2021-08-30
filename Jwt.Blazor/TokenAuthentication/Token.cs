using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt.Blazor.TokenAuthentication
{
    public class Token
    {
        public string Value { get; set; }
        public DateTime Expiry { get; set; }
    }
}
