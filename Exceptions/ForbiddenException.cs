using System.Net;

namespace EdAppTest.Exceptions
{
    public class ForbiddenException : HttpRequestException
    {
        public ForbiddenException(object entity, string message)
            : base($"Action on {entity} is forbidden:\n{message}")
        {
            HttpStatusCode = HttpStatusCode.Forbidden;
        }
    }
}