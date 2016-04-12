using Microsoft.AspNet.Mvc;

namespace Lisa.Common.WebApi.Test
{
    [Route("books")]
    public class BookController
    {
        [HttpGet]
        public ActionResult Get()
        {
            var books = _db.FetchBooks();
            return new HttpOkObjectResult(books);
        }

        [HttpPost]
        public ActionResult Post([FromBody] DynamicModel book)
        {
            if (book == null)
            {
                return new BadRequestResult();
            }

            var validator = new BookValidator();
            var validationResult = validator.Validate(book);
            if (validationResult.HasErrors)
            {
                return new UnprocessableEntityObjectResult(validationResult.Errors);
            }

            var result = _db.CreateBook(book);
            return new CreatedResult("", result);
        }

        private readonly BookDatabase _db = new BookDatabase();
    }

    public class BookValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("title");
            Required("author.firstName", NotEmpty);
            Required("author.lastName", MinLength(5));
        }
    }
}