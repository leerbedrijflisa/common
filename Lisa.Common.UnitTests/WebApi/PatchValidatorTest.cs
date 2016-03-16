using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class PatchValidatorTest
    {
        [Fact]
        public void ItReturnsSuccessWhenThereAreNoErrors()
        {
            dynamic model = new DynamicModel();
            model.Title = "A Clockwork Orange";

            var patch = new Patch
            {
                Action = "replace",
                Field = "title",
                Value = "Magician"
            };

            var validator = new BookValidator();
            var result = validator.Validate(new Patch[] { patch }, model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenAnActionIsInvalid()
        {
            dynamic model = new DynamicModel();
            model.Title = "A Clockwork Orange";

            var patch = new Patch
            {
                Action = "edit",
                Field = "title",
                Value = "Magician"
            };

            var validator = new BookValidator();
            ValidationResult result = validator.Validate(new Patch[] { patch }, model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidAction, error.Code);
            Assert.Equal("edit", AnonymousField(error.Values, "Action"));
        }

        [Fact]
        public void ItReportsWhenAFieldIsInvalid()
        {
            dynamic model = new DynamicModel();
            model.Title = "A Clockwork Orange";

            var patch = new Patch
            {
                Action = "replace",
                Field = "name",
                Value = "Magician"
            };

            var validator = new BookValidator();
            ValidationResult result = validator.Validate(new Patch[] { patch }, model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidField, error.Code);
            Assert.Equal("name", AnonymousField(error.Values, "Field"));
        }

        private object AnonymousField(object obj, string fieldName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(fieldName);
            return propertyInfo.GetValue(obj);
        }
    }
}