namespace GraphQL.GraphExceptions
{
    public abstract class BaseError
    {
        private readonly BaseException exception;

        public BaseError(BaseException exception)
        {
            this.exception = exception;
        }

        public string Message => exception.Message;

        public int Code => exception.Code;
    }
}