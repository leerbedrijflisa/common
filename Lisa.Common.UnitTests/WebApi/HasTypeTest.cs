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
            Assert.Equal("string", AnonymousField(error.Values, "Expected"));
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
            Assert.Equal("string", AnonymousField(error.Values, "Expected"));
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
            Assert.Equal("string", AnonymousField(error.Values, "Expected"));
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
            Assert.Equal("number", AnonymousField(error.Values, "Expected"));
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

        [Fact]
        public void ItAcceptsIfTypeMatchesAnyOfMany()
        {
            dynamic model = new DynamicModel();
            model.Age = "old";

            var validator = new IsOneOfManyValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItRejectsIfTypeMatchesNoneOfMany()
        {
            dynamic model = new DynamicModel();
            model.Age = true;

            var validator = new IsOneOfManyValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var accepted = (string[]) AnonymousField(error.Values, "Expected");
            Assert.Equal(ErrorCode.InvalidType, error.Code);
            Assert.Equal("Age", AnonymousField(error.Values, "Field"));
            Assert.Equal("boolean", AnonymousField(error.Values, "Actual"));
            Assert.Contains("string", accepted);
            Assert.Contains("number", accepted);
        }

        [Fact]
        public void ItAcceptsAnAnonymousObject()
        {
            dynamic model = new DynamicModel();
            model.Character = new
            {
                Name = "Rumpelstiltskin"
            };

            var validator = new IsObjectValidator();
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
            Optional("name", TypeOf(DataTypes.String));
        }
    }

    class IsNumberValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("age", TypeOf(DataTypes.Number));
        }
    }

    class IsBooleanValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("isNice", TypeOf(DataTypes.Boolean));
        }
    }

    class IsArrayValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("traits", TypeOf(DataTypes.Array));
        }
    }

    class IsOneOfManyValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("age", TypeOf(DataTypes.Number | DataTypes.String));
        }
    }

    class IsObjectValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("character", TypeOf(DataTypes.Object));
            Required("character.name");
        }
    }
}