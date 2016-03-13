using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;
using System;

namespace Lisa.Common.UnitTests
{
    public class ValidatorTest
    {
        [Fact]
        public void ItReturnsSuccessWhenThereAreNoErrors()
        {
            var validator = new EmptyValidator();
            var model = new DynamicModel();

            var result = validator.Validate(model);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenARequiredFieldIsMissing()
        {
            var validator = new BookValidator();
            dynamic book = new DynamicModel();
            book.Author = "Alexandre Dumas";

            ValidationResult result = validator.Validate(book);
            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.FieldMissing, error.Code);
            Assert.Equal("title", AnonymousField(error.Values, "Field"));
        }

        [Fact]
        public void ItIgnoresCaseOfRequiredFields()
        {
            var validator = new BookValidator();
            dynamic book = new DynamicModel();
            book.Title = "The Count of Monte-Cristo";

            ValidationResult result = validator.Validate(book);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItPreserversCaseOfFieldInErrorValues()
        {
            var validator = new PersonValidator();
            dynamic person = new DynamicModel();
            person.FirstName = "Alexandre";

            ValidationResult result = validator.Validate(person);
            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.FieldMissing, error.Code);
            Assert.Equal("lastName", AnonymousField(error.Values, "Field"));
        }

        [Fact]
        public void ItChecksForRequiredFieldsOnAnEmptyModel()
        {
            var validator = new BookValidator();
            dynamic book = new DynamicModel();

            ValidationResult result = validator.Validate(book);
            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.FieldMissing, error.Code);
            Assert.Equal("title", AnonymousField(error.Values, "Field"));
        }

        [Fact]
        public void ItReportsExtraFields()
        {
            var validator = new BookValidator();
            dynamic book = new DynamicModel();
            book.Title = "The Count of Monte-Cristo";
            book.Rating = 5;

            ValidationResult result = validator.Validate(book);
            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidField, error.Code);
            Assert.Equal("Rating", AnonymousField(error.Values, "Field"));
        }

        [Fact]
        public void ItRunsValidationFunctionsOnRequiredFields()
        {
            var validator = new BookValidator();
            dynamic book = new DynamicModel();
            book.Title = string.Empty;

            ValidationResult result = validator.Validate(book);
            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.EmptyValue, error.Code);
            Assert.Equal("Title", AnonymousField(error.Values, "Field"));
        }

        [Fact]
        public void ItRunsValidationFunctionsOnOptionalFields()
        {
            var validator = new BookValidator();
            dynamic book = new DynamicModel();
            book.Title = "The Count of Monte-Cristo";
            book.Author = string.Empty;

            ValidationResult result = validator.Validate(book);
            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.EmptyValue, error.Code);
            Assert.Equal("Author", AnonymousField(error.Values, "Field"));
        }

        private object AnonymousField(object obj, string fieldName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(fieldName);
            return propertyInfo.GetValue(obj);
        }
    }

    public class EmptyValidator : Validator
    {
        public override void ValidateModel()
        {
        }
    }

    public class BookValidator : Validator
    {
        public override void ValidateModel()
        {
            Required("title", NotEmpty);
            Optional("author", NotEmpty);
        }
    }

    public class PersonValidator : Validator
    {
        public override void ValidateModel()
        {
            Required("firstName");
            Required("lastName");
        }
    }
}