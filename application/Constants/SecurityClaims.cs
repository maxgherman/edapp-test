using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EdAppTest.Constants
{
    public static class SecurityClaims
    {
        public static readonly string UserIdentifier = ClaimTypes.NameIdentifier;

        public static readonly string JWTIdentifier = JwtRegisteredClaimNames.Jti;
    }
}