namespace Lisa.Common.WebApi.Test
{
    public static class BookMapper
    {
        public static DynamicModel ToModel(Book entity)
        {
            dynamic model = new DynamicModel();
            model.Title = entity.Title;
            model.Author = new
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName
            };

            return model;
        }

        public static Book ToEntity(dynamic model)
        {
            return new Book
            {
                Title = model.Title,
                FirstName = model.Author.FirstName,
                LastName = model.Author.LastName
            };
        }
    }
}