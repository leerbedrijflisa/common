﻿using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;

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

            var error = result.Errors.First();
            dynamic values = error.Values;
            Assert.Equal(ErrorCode.FieldMissing, error.Code);
            Assert.Equal("title", AnonymousField(values, "Field"));
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
            Required("title");
        }
    }
}