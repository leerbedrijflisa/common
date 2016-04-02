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
            model.Name = "Rumpelstiltskin";

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
            model.Name = new string[] { "Rumpelstiltskin" };

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

        [Fact]
        public void ItAcceptsANumber()
        {
            dynamic model = new DynamicModel();
            model.Age = 325;

            var validator = new IsNumberValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItAcceptsABool()
        {
            dynamic model = new DynamicModel();
            model.IsNice = false;

            var validator = new IsBooleanValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItAcceptsAnArray()
        {
            dynamic model = new DynamicModel();
            model.Traits = new string[] { "secretive", "conniving" };

            var validator = new IsArrayValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItRejectsAStringAsANumber()
        {
            dynamic model = new DynamicModel();
            model.Age = "old";

            var validator = new IsNumberValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidType, error.Code);
            Assert.Equal("Age", AnonymousField(error.Values, "Field"));
            Assert.Equal("number", AnonymousField(error.Values, "Accepted"));
            Assert.Equal("string", AnonymousField(error.Values, "Actual"));
        }

        [Fact]
        public void ItIgnoresNull()
        {
            dynamic model = new DynamicModel();
            model.Name = null;

            var validator = new IsStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
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

    class IsNumberValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("age", HasType(DataType.Number));
        }
    }

    class IsBooleanValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("isNice", HasType(DataType.Boolean));
        }
    }

    class IsArrayValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("traits", HasType(DataType.Array));
        }
    }
}