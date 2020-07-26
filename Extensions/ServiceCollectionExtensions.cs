using System;
using System.Text;
using EdAppTest.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EdAppTest.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var appOptions = configuration.ParseValue<AppOptions>("App");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.FromSeconds(1),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = appOptions.ClientId,
                        ValidAudience = appOptions.ClientId,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(appOptions.ClientSecret)
                        ),
                        LifetimeValidator = (
                            DateTime? notBefore,
                            DateTime? expires,
                            SecurityToken token,
                            TokenValidationParameters @params) =>
                        {
                            if (expires.HasValue)
                            {
                                var value = DateTime.SpecifyKind(expires.Value, DateTimeKind.Utc);
                                return value > DateTime.UtcNow;
                            }

                            return false;
                        }
                    };
                });
        }
    }
}
