using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Active_Blog_Service_API.Helpers
{
    public class JwtHelper
    {
        private static string GetToken(string authHeader)
        {
            string token = "";
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                // Extract the token by removing "Bearer "
                token = authHeader.Substring("Bearer ".Length).Trim();
                
            }
            return token;
        }
        public static IEnumerable<Claim> GetClaimsFromJwt(string authHeader)
        {
            string token = GetToken(authHeader);

            var handler = new JwtSecurityTokenHandler();

            // Ensure the token is a valid JWT
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);

                // Extract the claims from the token
                var claims = jwtToken.Claims;

                // Iterate over the claims to display key-value pairs
                return claims;
            }
            return Enumerable.Empty<Claim>();
        }

    }
}
