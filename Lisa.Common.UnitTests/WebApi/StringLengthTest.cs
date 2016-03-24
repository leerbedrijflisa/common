using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;
using System;

namespace Lisa.Common.UnitTests.WebApi
{
    public class StringLengthTest
    {
        [Fact]
        public void ItSucceedsWhenLengthIsEqual()
        {
            dynamic code = new DynamicModel();
            code.Letters = "word";

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenLengthIsNotEqual()
        {
            dynamic code = new DynamicModel();
            code.Letters = "not";

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.InvalidLength, error.Code);
            Assert.Equal("Letters", AnonymousField(error.Values, "Field"));
            Assert.Equal(4, AnonymousField(error.Values, "Expected"));
            Assert.Equal(3, AnonymousField(error.Values, "Actual"));
        }

        [Fact]
        public void ItIgnoresANullValueOnLength()
        {
            dynamic code = new DynamicModel();
            code.Letters = null;

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItIgnoresAValueOfInvalidTypeOnLength()
        {
            dynamic code = new DynamicModel();
            code.Letters = 15;

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenLengthIsMinimum()
        {
            dynamic code = new DynamicModel();
            code.Characters = "min";

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenLengthIsAboveMinimum()
        {
            dynamic code = new DynamicModel();
            code.Characters = "word";

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenLengthIsBelowMinimum()
        {
            dynamic code = new DynamicModel();
            code.Characters = "no";

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.TooShort, error.Code);
            Assert.Equal("Characters", AnonymousField(error.Values, "Field"));
            Assert.Equal(3, AnonymousField(error.Values, "Minimum"));
            Assert.Equal(2, AnonymousField(error.Values, "Actual"));
        }

        [Fact]
        public void ItIgnoresANullValueOnMinLength()
        {
            dynamic code = new DynamicModel();
            code.Characters = null;

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItIgnoresAValueOfInvalidTypeOnMinLength()
        {
            dynamic code = new DynamicModel();
            code.Characters = 15;

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenLengthIsMaximum()
        {
            dynamic code = new DynamicModel();
            code.Glyphs = "ok";

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenLengthIsBelowMaximum()
        {
            dynamic code = new DynamicModel();
            code.Glyphs = "I";

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenLengthIsAboveMaximum()
        {
            dynamic code = new DynamicModel();
            code.Glyphs = "long";

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            Assert.Equal(ErrorCode.TooLong, error.Code);
            Assert.Equal("Glyphs", AnonymousField(error.Values, "Field"));
            Assert.Equal(2, AnonymousField(error.Values, "Maximum"));
            Assert.Equal(4, AnonymousField(error.Values, "Actual"));
        }

        [Fact]
        public void ItIgnoresANullValueOnMaxLength()
        {
            dynamic code = new DynamicModel();
            code.Glyphs = null;

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItIgnoresAValueOfInvalidTypeOnMaxLength()
        {
            dynamic code = new DynamicModel();
            code.Glyphs = 15;

            var validator = new WordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItSucceedsWhenOneOfTheSpecifiedLengthsIsEqual()
        {
            dynamic code = new DynamicModel();
            code.Letters = "multiple";

            var validator = new MultiwordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsWhenNoneOfTheSpecifiedLengthsIsEqual()
        {
            dynamic code = new DynamicModel();
            code.Letters = "no";

            var validator = new MultiwordValidator();
            ValidationResult result = validator.Validate(code);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var expected = (int[]) AnonymousField(error.Values, "Expected");
            Assert.Equal(ErrorCode.InvalidLength, error.Code);
            Assert.Equal("Letters", AnonymousField(error.Values, "Field"));
            Assert.Equal(2, AnonymousField(error.Values, "Actual"));
            Assert.Contains(5, expected);
            Assert.Contains(8, expected);
            Assert.Contains(13, expected);
        }

        private object AnonymousField(object obj, string fieldName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(fieldName);
            return propertyInfo.GetValue(obj);
        }
    }

    public class WordValidator : Validator
    {
        protected override void ValidateModel()
        {
            Optional("letters", Length(4));
            Optional("characters", MinLength(3));
            Optional("glyphs", MaxLength(2));
        }
    }

    public class MultiwordValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("letters", Length(5, 8, 13));
        }
    }
}