using System;

namespace GamestoreApi.Middleware
{
    public interface IJwtValidator
    {
        bool TryValidateToken(string token, out System.Security.Claims.ClaimsPrincipal? principal);
    }
}
