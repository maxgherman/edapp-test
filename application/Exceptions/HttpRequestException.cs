using System;
using System.Net;

namespace EdAppTest.Exceptions
{
    public interface IHttpRequestException
    {
        HttpStatusCode HttpStatusCode { get; set; }
    }

    public abstract class HttpRequestException : Exception, IHttpRequestException
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        protected HttpRequestException(string message) : base(message)
        { }
    }
}