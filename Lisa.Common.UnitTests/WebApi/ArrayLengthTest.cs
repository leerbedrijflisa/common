using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class ArrayLengthTest
    {
        [Fact]
        public void ItSucceedsWhenLengthIsEqual()
        {
            dynamic code = new DynamicModel();
            code.Digits = new int[] { 3, 4, 5, 2 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenLengthIsNotEqual()
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

        [Fact]
        public void ItIgnoresANullValueOnLength()
        {
            dynamic code = new DynamicModel();
            code.Digits = null;

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItIgnoresAValueOfInvalidTypeOnLength()
        {
            dynamic code = new DynamicModel();
            code.Digits = 15;

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenLengthIsMinimum()
        {
            dynamic code = new DynamicModel();
            code.Numbers = new int[] { 3, 5, 2 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenLengthIsAboveMinimum()
        {
            dynamic code = new DynamicModel();
            code.Numbers = new int[] { 3, 5, 2, 9 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenLengthIsBelowMinimum()
        {
            dynamic code = new DynamicModel();
            code.Numbers = new int[] { 2, 9 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.TooShort, error.Code);
            Assert.Equal("Numbers", AnonymousField(error.Values, "Field"));
            Assert.Equal(3, AnonymousField(error.Values, "Minimum"));
            Assert.Equal(2, AnonymousField(error.Values, "Actual"));
        }

        [Fact]
        public void ItIgnoresANullValueOnMinLength()
        {
            dynamic code = new DynamicModel();
            code.Numbers = null;

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItIgnoresAValueOfInvalidTypeOnMinLength()
        {
            dynamic code = new DynamicModel();
            code.Numbers = 15;

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenLengthIsMaximum()
        {
            dynamic code = new DynamicModel();
            code.Elements = new int[] { 3, 5 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenLengthIsBelowMaximum()
        {
            dynamic code = new DynamicModel();
            code.Elements = new int[] { 3 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenLengthIsAboveMaximum()
        {
            dynamic code = new DynamicModel();
            code.Elements = new int[] { 2, 9, 7, 6 };

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.TooLong, error.Code);
            Assert.Equal("Elements", AnonymousField(error.Values, "Field"));
            Assert.Equal(2, AnonymousField(error.Values, "Maximum"));
            Assert.Equal(4, AnonymousField(error.Values, "Actual"));
        }

        [Fact]
        public void ItIgnoresANullValueOnMaxLength()
        {
            dynamic code = new DynamicModel();
            code.Elements = null;

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItIgnoresAValueOfInvalidTypeOnMaxLength()
        {
            dynamic code = new DynamicModel();
            code.Elements = 15;

            var validator = new CodeValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
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
            Optional("digits", Length(4));
            Optional("numbers", MinLength(3));
            Optional("elements", MaxLength(2));
        }
    }
}