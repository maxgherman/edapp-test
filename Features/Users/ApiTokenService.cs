
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EdAppTest.Constants;
using EdAppTest.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EdAppTest.Features.Users
{
    public interface IApiTokenService
    {
        GenerateTokenResult GenerateToken(GenerateTokenRequest request);

        RefreshTokenResult RefreshToken(string token);
    }

    public class ApiTokenService : IApiTokenService
    {
        private readonly string securityAlgorithm = SecurityAlgorithms.HmacSha256Signature;
        private readonly AppOptions appOptions;
       
        public ApiTokenService(IConfiguration configuration)
        {
            appOptions = configuration.ParseValue<AppOptions>("App");
        }

        public GenerateTokenResult GenerateToken(GenerateTokenRequest request)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appOptions.ClientSecret));
            var credentials = new SigningCredentials(securityKey, securityAlgorithm);

            var claims = new[] {
                new Claim(SecurityClaims.UserIdentifier, request.UserId),
                new Claim(SecurityClaims.JWTIdentifier, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
              appOptions.ClientId,
              appOptions.ClientId,
              claims,
              expires: DateTime.UtcNow.AddMinutes(appOptions.TokenExpirationMinutes),
              signingCredentials: credentials);

            return new GenerateTokenResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = GenerateRefreshToken()
            };
        }

        public RefreshTokenResult RefreshToken(string token)
        {
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = appOptions.ClientId,
                ValidAudience = appOptions.ClientId,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(appOptions.ClientSecret)
                )
            };

            var principal = default(ClaimsPrincipal);
            var securityToken = default(SecurityToken);

            try
            {
                principal = new JwtSecurityTokenHandler().ValidateToken(
                    token, tokenValidationParams, out securityToken
                );
            }
            catch (Exception ex)
            {
                //TODO: log exception
                return new RefreshTokenResult { Success = false, Message = "Invalid token" };
            }

            var jwtToken = securityToken as JwtSecurityToken;

            if (jwtToken == null ||
                !string.Equals(jwtToken.Header.Alg, securityAlgorithm, StringComparison.OrdinalIgnoreCase))
            {
                return new RefreshTokenResult { Success = false, Message = "Invalid token" };
            }

            var userId = principal.FindFirstValue(SecurityClaims.UserIdentifier);
            
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new RefreshTokenResult { Success = false, Message = "Invalid token" };
            }

            var result = GenerateToken(new GenerateTokenRequest
            {
                UserId = userId,
            });

            return new RefreshTokenResult
            {
                Success = true,
                UserId = userId,
                Result = result
            };
        }

        private RefreshToken GenerateRefreshToken()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                cryptoProvider.GetBytes(randomBytes);
                
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddMinutes(
                        appOptions.RefreshTokenExpirationMinutes)
                };
            }
        }
    }

    public class GenerateTokenRequest
    {
        public string UserId { get; set; }
    }

    public class GenerateTokenResult
    {
        public string Token { get; set; }

        public RefreshToken RefreshToken { get; set; }
    }

    public class RefreshTokenResult
    {
        public bool Success { get; set; }

        public string UserId { get; set; }

        public string Message { get; set; }

        public GenerateTokenResult Result { get; set; }
    }

    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }
    }
}