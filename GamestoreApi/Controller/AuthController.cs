using Microsoft.AspNetCore.Mvc;
using GamestoreApi.Service;
using Microsoft.AspNetCore.Authorization;

namespace GamestoreApi.Controller
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class AuthController(ITokenGenerator tokenGenerator) : ControllerBase
    {
        /// <summary>
        /// Dev-only endpoint: Generate a test JWT token.
        /// POST /v1/api/auth/token
        /// </summary>
        /// <remarks>
        /// This endpoint is for development and testing only.
        /// It generates a valid JWT token signed with the configured secret.
        /// Use this to test authenticated endpoints without a real login system.
        /// </remarks>
        /// <param name="request">Token request with optional username (defaults to "testuser")</param>
        /// <returns>Token response with JWT and expiration time</returns>
        [HttpPost("token")]
        [AllowAnonymous]
        public IActionResult GenerateToken([FromBody] TokenRequest? request)
        {
            var subject = request?.Username ?? "testuser";
            var expiresInSeconds = request?.ExpiresInSeconds ?? 3600;

            var token = tokenGenerator.GenerateToken(subject, expiresInSeconds);
            return Ok(new { token, expiresIn = expiresInSeconds });
        }
    }

    public class TokenRequest
    {
        public string? Username { get; set; }
        public int ExpiresInSeconds { get; set; } = 3600;
    }
}
