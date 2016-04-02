using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class HasTypeTest
    {
        [Fact]
        public void ItAcceptsAString()
        {
            dynamic model = new DynamicModel();
            model.Name = "Rimpelstiltskin";

            var validator = new IsStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItRejectsANumberAsAString()
        {
            dynamic model = new DynamicModel();
            model.Name = 10.5;

            var validator = new IsStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidType, error.Code);
            Assert.Equal("Name", AnonymousField(error.Values, "Field"));
            Assert.Equal("string", AnonymousField(error.Values, "Accepted"));
            Assert.Equal("number", AnonymousField(error.Values, "Actual"));
        }

        [Fact]
        public void ItRejectsABoolAsAString()
        {
            dynamic model = new DynamicModel();
            model.Name = true;

            var validator = new IsStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidType, error.Code);
            Assert.Equal("Name", AnonymousField(error.Values, "Field"));
            Assert.Equal("string", AnonymousField(error.Values, "Accepted"));
            Assert.Equal("boolean", AnonymousField(error.Values, "Actual"));
        }

        [Fact]
        public void ItRejectsAnArrayAsAString()
        {
            dynamic model = new DynamicModel();
            model.Name = new string[] { "Rimpelstiltskin" };

            var validator = new IsStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidType, error.Code);
            Assert.Equal("Name", AnonymousField(error.Values, "Field"));
            Assert.Equal("string", AnonymousField(error.Values, "Accepted"));
            Assert.Equal("array", AnonymousField(error.Values, "Actual"));
        }

        private object AnonymousField(object obj, string fieldName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(fieldName);
            return propertyInfo.GetValue(obj);
        }
    }

    class IsStringValidator : Validator
    {
        protected override void ValidateModel()
        {
            Optional("name", HasType(DataType.String));
        }
    }
}