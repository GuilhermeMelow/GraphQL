
namespace GraphQL.GraphExceptions.Book
{
    public class DivergentGuidError : BaseError
    {
        public DivergentGuidError(BaseException exception) : base(exception) { }
    }

    public class BookNotFoundError : BaseError
    {
        public BookNotFoundError(BaseException exception) : base(exception) { }
    }

    public class BookErrorFactory :
            IPayloadErrorFactory<DivergentGuidException, DivergentGuidError>,
            IPayloadErrorFactory<BookNotFoundException, BookNotFoundError>
    {
        public DivergentGuidError CreateErrorFrom(DivergentGuidException ex) => new(ex);

        public BookNotFoundError CreateErrorFrom(BookNotFoundException ex) => new(ex);
    }
}