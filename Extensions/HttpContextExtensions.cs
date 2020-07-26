using System;
using System.Security.Claims;
using EdAppTest.Constants;
using Microsoft.AspNetCore.Http;

namespace EdAppTest.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid UserId(this HttpContext context)
        {
            return Guid.Parse(context.User.FindFirstValue(SecurityClaims.UserIdentifier));
        }
    }
}