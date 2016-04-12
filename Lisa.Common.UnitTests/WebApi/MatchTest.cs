using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class MatchTest
    {
        [Fact]
        public void ItAcceptsAMatchingString()
        {
            dynamic model = new DynamicModel();
            model.Email = "fine@example.com";

            var validator = new MatchValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItRejectsANonMatchingString()
        {
            dynamic model = new DynamicModel();
            model.Email = "@example";

            var validator = new MatchValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.NoMatch, error.Code);
            Assert.Equal("Email", AnonymousField(error.Values, "Field"));
            Assert.Equal("@example", AnonymousField(error.Values, "Value"));
            Assert.Equal(@"^.+@.+\..+", AnonymousField(error.Values, "Pattern"));
        }

        [Fact]
        public void ItIgnoresNull()
        {
            dynamic model = new DynamicModel();
            model.Email = null;

            var validator = new MatchValidator();
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

    class MatchValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("email", Match(@"^.+@.+\..+"));
        }
    }
}