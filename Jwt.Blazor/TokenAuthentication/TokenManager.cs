using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt.Blazor.TokenAuthentication
{
    public class TokenManager
    {
        public bool Authenticate(string un, string pw)
        {
            // Here we can go to JwtServer --> TokenController/ and ask it to authenticate the un/pw.
            // It returns
            return true;
        }

        public Token NewToken()
        {
            // Here we can go to JwtServer and get a new token
            return null;
        }

        public bool VerifyToken(string token)
        {
            // locally
            return false;
        }
    }
}
