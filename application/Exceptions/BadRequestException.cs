using System.Net;

namespace EdAppTest.Exceptions
{
    public class BadRequestException : HttpRequestException
    {
        public BadRequestException(object entity, string message)
            : base($"Entity {entity} has bad or invalid data:\n{message}")
        {
            HttpStatusCode = HttpStatusCode.BadRequest;
        }
    }
}