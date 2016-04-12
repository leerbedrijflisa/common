using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class IsArrayTest
    {
        [Fact]
        public void ItAcceptsAnArrayOfStrings()
        {
            dynamic model = new DynamicModel();
            model.Directions = new[] { "up", "down", "left", "right" };

            var validator = new ArrayOfStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItRejectsAnArrayOfInvalidTypes()
        {
            dynamic model = new DynamicModel();
            model.Directions = new[] { 1, 2, 3, 4 };

            var validator = new ArrayOfStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(4, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidType, error.Code);
            Assert.Equal("Directions", AnonymousField(error.Values, "Field"));
            Assert.Equal("string", AnonymousField(error.Values, "Expected"));
            Assert.Equal("number", AnonymousField(error.Values, "Actual"));
            Assert.Equal(1, AnonymousField(result.Errors.ElementAt(0).Values, "Value"));
            Assert.Equal(2, AnonymousField(result.Errors.ElementAt(1).Values, "Value"));
            Assert.Equal(3, AnonymousField(result.Errors.ElementAt(2).Values, "Value"));
            Assert.Equal(4, AnonymousField(result.Errors.ElementAt(3).Values, "Value"));
        }

        [Fact]
        public void ItRejectsAnArrayWithAnInvalidType()
        {
            dynamic model = new DynamicModel();
            model.Directions = new object[] { "up", 2 };

            var validator = new ArrayOfStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidType, error.Code);
            Assert.Equal("Directions", AnonymousField(error.Values, "Field"));
            Assert.Equal("string", AnonymousField(error.Values, "Expected"));
            Assert.Equal("number", AnonymousField(error.Values, "Actual"));
            Assert.Equal(2, AnonymousField(error.Values, "Value"));
        }

        private object AnonymousField(object obj, string fieldName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(fieldName);
            return propertyInfo.GetValue(obj);
        }
    }

    class ArrayOfStringValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("directions", IsArray(DataTypes.String));
        }
    }
}