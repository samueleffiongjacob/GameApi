using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GamestoreApi.Middleware
{
    public class GameStoreJwtValidator : IJwtValidator
    {
        private readonly IConfiguration _config;
        private readonly ILogger<GameStoreJwtValidator> _logger;

        public GameStoreJwtValidator(IConfiguration config, ILogger<GameStoreJwtValidator> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool TryValidateToken(string token, out System.Security.Claims.ClaimsPrincipal? principal)
        {
            principal = null;

            var secret = _config["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
            {
                _logger.LogWarning("JWT secret is not configured.");
                return false;
            }

            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var handler = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                principal = handler.ValidateToken(token, parameters, out _);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "JWT validation failed");
                return false;
            }
        }
    }
}
