using Lisa.Common.WebApi;
using Newtonsoft.Json.Linq;
using System;
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
            book.tItLe = "The Count of Monte-Cristo";

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
            Assert.Equal(ErrorCode.ExtraField, error.Code);
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

        [Fact]
        public void ItThrowsWhenAnOptionalFieldGetsMarkedRequired()
        {
            var validator = new ParadoxValidator();
            var paradox = new DynamicModel();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(paradox));
        }

        [Fact]
        public void ItThrowsWhenARequiredFieldGetsMarkedOptional()
        {
            var validator = new ContradictionValidator();
            var contradiction = new DynamicModel();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(contradiction));
        }

        [Fact]
        public void ItThrowsWhenAccessingModelOutsideRequiredAndOptional()
        {
            var model = new DynamicModel();
            var validator = new NastyModelValidator();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(model));
        }

        [Fact]
        public void ItThrowsWhenAccessingResultOutsideRequiredAndOptional()
        {
            var model = new DynamicModel();
            var validator = new NastyResultValidator();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(model));
        }

        [Fact]
        public void ItThrowsWhenAccessingPropertyOutsideRequiredAndOptional()
        {
            var model = new DynamicModel();
            var validator = new NastyPropertyValidator();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(model));
        }

        [Fact]
        public void ItThrowsWhenAccessingPatchOutsideRequiredAndOptional()
        {
            var model = new DynamicModel();
            var validator = new NastyPatchValidator();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(model));
        }

        [Fact]
        public void ItSucceedsWhenIgnoredFieldIsSpecified()
        {
            dynamic model = new DynamicModel();
            model.Registered = DateTime.UtcNow;
            model.FirstName = "Alexandre";
            model.LastName = "Dumas";

            var validator = new PersonValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenIgnoredFieldIsMissing()
        {
            dynamic model = new DynamicModel();
            model.FirstName = "Alexandre";
            model.LastName = "Dumas";

            var validator = new PersonValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItThrowsWhenAnIgnoredFieldGetsMarkedRequired()
        {
            var validator = new IgnoreRequiredValidator();
            var model = new DynamicModel();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(model));
        }

        [Fact]
        public void ItThrowsWhenAnIgnoredFieldGetsMarkedOptional()
        {
            var validator = new IgnoreOptionalValidator();
            var model = new DynamicModel();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(model));
        }

        [Fact]
        public void ItThrowsWhenAnRequiredFieldGetsMarkedIgnored()
        {
            var validator = new RequiredIgnoreValidator();
            var model = new DynamicModel();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(model));
        }

        [Fact]
        public void ItThrowsWhenAnOptionalFieldGetsMarkedIgnored()
        {
            var validator = new OptionalIgnoreValidator();
            var model = new DynamicModel();

            Assert.Throws<InvalidOperationException>(() => validator.Validate(model));
        }

        [Fact]
        public void ItSucceedsWhenNestedFieldIsSpecified()
        {
            var validator = new NestedValidator();
            dynamic model = new DynamicModel();
            model.User = new
            {
                Name = new
                {
                    First = "Alexandre"
                }
            };

            ValidationResult result = validator.Validate(model);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenNestedJsonFieldIsSpecified()
        {
            var validator = new NestedValidator();
            dynamic model = new DynamicModel();
            model.User = JObject.Parse("{ name: { first: 'Alexandre' } }");

            ValidationResult result = validator.Validate(model);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenNestedFieldIsNull()
        {
            var validator = new NestedValidator();
            dynamic model = new DynamicModel();
            model.User = JObject.Parse("{ name: { first: null } }");

            ValidationResult result = validator.Validate(model);
            Assert.False(result.HasErrors);
            Assert.Null(model.User.name.first);
        }

        [Fact]
        public void ItRunsValidationFunctionsOnFieldsNestedInAnArray()
        {
            var validator = new AuthorValidator();
            dynamic model = new DynamicModel();
            model.Authors = new[]
            {
                new { FirstName = "Alexandre", LastName = "Dumas" },
                new { FirstName = "Anthony", LastName = "Burgess" },
                new { FirstName = "Victor", LastName = "Hugo" }
            };

            ValidationResult result = validator.Validate(model);
            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.TooShort, error.Code);
        }

        [Fact]
        public void ItRunsValidationFunctionsOnNestedDynamicModels()
        {
            var validator = new AuthorValidator();
            dynamic model = new DynamicModel();
            dynamic author = new DynamicModel();
            author.FirstName = "Victor";
            author.LastName = "Hugo";
            model.Authors = new[] { author };

            ValidationResult result = validator.Validate(model);
            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.TooShort, error.Code);
        }

        [Fact]
        public void ItAcceptsNullValuesInArrays()
        {
            dynamic model = new DynamicModel();
            model.Numbers = new object[] { 3, 2, 1, null };

            var validator = new ArrayValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        private object AnonymousField(object obj, string fieldName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(fieldName);
            return propertyInfo?.GetValue(obj);
        }
    }

    public class EmptyValidator : Validator
    {
        protected override void ValidateModel()
        {
        }
    }

    public class BookValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("title", NotEmpty);
            Optional("author", NotEmpty);
        }

        protected override void ValidatePatch()
        {
            Allow("title");
        }
    }

    public class PersonValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("registered");
            Required("firstName");
            Required("lastName");
        }
    }

    public class ParadoxValidator : Validator
    {
        protected override void ValidateModel()
        {
            Optional("truth");
            Required("truth");
        }
    }

    public class ContradictionValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("fact");
            Optional("fact");
        }
    }

    public class IgnoreRequiredValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("this");
            Required("this");
        }
    }

    public class IgnoreOptionalValidator : Validator
    {
        protected override void ValidateModel()
        {
            Ignore("this");
            Optional("this");
        }
    }

    public class RequiredIgnoreValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("this");
            Ignore("this");
        }
    }

    public class OptionalIgnoreValidator : Validator
    {
        protected override void ValidateModel()
        {
            Optional("this");
            Ignore("this");
        }
    }

    public class NastyModelValidator : Validator
    {
        protected override void ValidateModel()
        {
            var model = Model;
        }
    }

    public class NastyPropertyValidator : Validator
    {
        protected override void ValidateModel()
        {
            var property = Property;
        }
    }

    public class NastyPatchValidator : Validator
    {
        protected override void ValidateModel()
        {
            var patch = Patch;
        }
    }

    public class NastyResultValidator : Validator
    {
        protected override void ValidateModel()
        {
            var result = Result;
        }
    }

    public class NestedValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("user.name.first");
        }

        protected override void ValidatePatch()
        {
            Allow("user.name.first");
        }
    }

    public class AuthorValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("authors");
            Optional("authors.firstName");
            Required("authors.lastName", MinLength(5));
        }
    }

    public class ArrayValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("numbers");
        }
    }
}