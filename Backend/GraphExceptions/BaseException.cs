using System.Net;

namespace GraphQL.GraphExceptions
{
    public abstract class BaseException : Exception
    {
        public BaseException(string message, HttpStatusCode code) : base(message)
        {
            this.code = code;
        }

        private readonly HttpStatusCode code;
        public int Code => ((int)code);
    }
}