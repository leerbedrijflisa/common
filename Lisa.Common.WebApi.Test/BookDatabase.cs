using System.Collections.Generic;
using System.Linq;

namespace Lisa.Common.WebApi.Test
{
    public class BookDatabase
    {
        static BookDatabase()
        {
            _books.Add(new Book
            {
                Title = "The Count Of Monte-Cristo",
                FirstName = "Alexandre",
                LastName = "Dumas"
            });
        }

        public IEnumerable<DynamicModel> FetchBooks()
        {
            return _books.Select(BookMapper.ToModel);
        }

        public DynamicModel CreateBook(DynamicModel book)
        {
            Book entity = BookMapper.ToEntity(book);
            _books.Add(entity);

            return BookMapper.ToModel(entity);
        }

        private static List<Book> _books = new List<Book>();
    }

    public class Book
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}