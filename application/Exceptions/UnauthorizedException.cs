using System.Net;

namespace EdAppTest.Exceptions
{
    public class UnauthorizedException : HttpRequestException
    {
        public UnauthorizedException(object entity, string message)
            : base($"Action on {entity} is not authorized:\n{message}")
        {
            HttpStatusCode = HttpStatusCode.Unauthorized;
        }
    }
}