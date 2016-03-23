using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class LengthTest
    {
        [Fact]
        public void ItSucceedsWhenArrayLengthIsEqual()
        {
            dynamic code = new DynamicModel();
            code.Digits = new int[] { 3, 4, 5, 2 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenArrayLengthIsNotEqual()
        {
            dynamic code = new DynamicModel();
            code.Digits = new int[] { 9, 3, 0 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidLength, error.Code);
            Assert.Equal("Digits", AnonymousField(error.Values, "Field"));
            Assert.Equal(4, AnonymousField(error.Values, "Expected"));
            Assert.Equal(3, AnonymousField(error.Values, "Actual"));
        }

        private object AnonymousField(object obj, string fieldName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(fieldName);
            return propertyInfo.GetValue(obj);
        }
    }

    public class CodeValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("digits", Length(4));
        }
    }
}