using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Jwt.Blazor.Helpers
{
    public static class Utils
    {
        public static string ShortenJWT(string token)
        {
            // Pass back the first 3 & last 10 digits of the Token
            if (!string.IsNullOrWhiteSpace(token))
                return $"{token.Substring(0, 3)}...{token.Substring(token.Length - 10, 10)}";
            return "no token!";
        }
    }
}
