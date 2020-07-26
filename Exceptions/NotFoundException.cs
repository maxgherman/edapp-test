using System.Net;

namespace EdAppTest.Exceptions
{
    public class NotFoundException : HttpRequestException
    {
        public NotFoundException(object entity, string key) :
            base($"Entity {entity} with key : {key} was not found.")
        {
            HttpStatusCode = HttpStatusCode.NotFound;
        }
    }
}