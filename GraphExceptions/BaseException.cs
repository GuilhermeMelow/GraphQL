using System.Net;

namespace GraphQL.GraphExceptions
{
    public abstract class BaseException : Exception
    {
        public BaseException(string message, HttpStatusCode code) : base(message)
        {
            _code = code;
        }

        private readonly HttpStatusCode _code;
        public int Code => ((int)_code);
    }
}