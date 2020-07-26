using Microsoft.Extensions.Configuration;

namespace EdAppTest.Extensions
{
    public static class ConfigurationExtensions
    {
        public static R ParseValue<R>(this IConfiguration configuration, string key)
        {
            return configuration.GetSection(key).Get<R>();
        }
    }
}