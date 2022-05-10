using System.Net;

namespace GraphQL.GraphExceptions.Book
{
    public class DivergentGuidException : BaseException
    {
        public DivergentGuidException(string message = "Há divergencia entre os Id's passados.") : base(message, HttpStatusCode.BadRequest)
        { }
    }

    public class BookAlredyRegisteredException : BaseException
    {
        public BookAlredyRegisteredException(string message = "O livro já foi registrado.") : base(message, HttpStatusCode.BadRequest)
        { }
    }

    public class BookNotFoundException : BaseException
    {
        public BookNotFoundException(string message = "Não foi possível encontrar o livro.") : base(message, HttpStatusCode.NotFound)
        { }
    }
}
