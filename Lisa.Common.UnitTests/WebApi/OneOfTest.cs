using Lisa.Common.WebApi;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class OneOfTest
    {
        [Fact]
        public void ItAcceptsAMatchingSingleStringValue()
        {
            dynamic model = new DynamicModel();
            model.Rating = "good";

            var validator = new OneOfStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsANonMatchingSingleStringValue()
        {
            dynamic model = new DynamicModel();
            model.Rating = "pass";

            var validator = new OneOfStringValidator();
            ValidationResult result = validator.Validate((DynamicModel) model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var values = (string[]) AnonymousField(error.Values, "Allowed");
            Assert.Equal(ErrorCode.IncorrectValue, error.Code);
            Assert.Contains("good", values);
            Assert.Contains("okay", values);
            Assert.Contains("bad", values);
        }

        [Fact]
        public void ItIgnoresANullValueForStringMatches()
        {
            dynamic model = new DynamicModel();
            model.Rating = null;

            var validator = new OneOfStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsMatchingANonStringAgainstAnArrayOfStrings()
        {
            dynamic model = new DynamicModel();
            model.Rating = 5;

            var validator = new OneOfStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var values = (string[]) AnonymousField(error.Values, "Allowed");
            Assert.Equal(ErrorCode.IncorrectValue, error.Code);
            Assert.Contains("good", values);
            Assert.Contains("okay", values);
            Assert.Contains("bad", values);
        }

        [Fact]
        public void ItAcceptsAMatchingDoubleValue()
        {
            dynamic model = new DynamicModel();
            model.Quarters = 0.75;

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsANonMatchingDoubleValue()
        {
            dynamic model = new DynamicModel();
            model.Quarters = 0.15;

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var values = (double[]) AnonymousField(error.Values, "Allowed");
            Assert.Equal(ErrorCode.IncorrectValue, error.Code);
            Assert.Equal("Quarters", AnonymousField(error.Values, "Field"));
            Assert.Equal(0.15, AnonymousField(error.Values, "Actual"));
            Assert.Contains(0.25, values);
            Assert.Contains(0.5, values);
            Assert.Contains(0.75, values);
            Assert.Contains(1, values);
        }

        [Fact]
        public void ItIgnoresANullValueForDoubleMatches()
        {
            dynamic model = new DynamicModel();
            model.Quarters = null;

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItAcceptsAMatchingFloatValue()
        {
            dynamic model = new DynamicModel();
            model.Quarters = 0.25f;

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItAcceptsAMatchingIntValue()
        {
            dynamic model = new DynamicModel();
            model.Quarters = 1;

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItAcceptsAMatchingDecimalValue()
        {
            dynamic model = new DynamicModel();
            model.Quarters = 0.50m;

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsMatchingANonNumberAgainstAnArrayOfDoubles()
        {
            dynamic model = new DynamicModel();
            model.Quarters = "three";

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var values = (double[]) AnonymousField(error.Values, "Allowed");
            Assert.Equal(ErrorCode.IncorrectValue, error.Code);
            Assert.Equal("Quarters", AnonymousField(error.Values, "Field"));
            Assert.Equal("three", AnonymousField(error.Values, "Actual"));
            Assert.Contains(0.25, values);
            Assert.Contains(0.5, values);
            Assert.Contains(0.75, values);
            Assert.Contains(1, values);
        }

        [Fact]
        public void ItAcceptsAnArrayOfMatchingStrings()
        {
            dynamic model = new DynamicModel();
            model.Rating = new string[] { "good", "bad", "good", "okay", "bad" };

            var validator = new OneOfStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsANonMatchingStringInAnArray()
        {
            dynamic model = new DynamicModel();
            model.Rating = new[] { "good", "pass", "bad" };

            var validator = new OneOfStringValidator();
            ValidationResult result = validator.Validate((DynamicModel) model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var values = (string[]) AnonymousField(error.Values, "Allowed");
            Assert.Equal(ErrorCode.IncorrectValue, error.Code);
            Assert.Equal("pass", AnonymousField(error.Values, "Actual"));
            Assert.Contains("good", values);
            Assert.Contains("okay", values);
            Assert.Contains("bad", values);
        }

        [Fact]
        public void ItAcceptsAnArrayOfMatchingNumbers()
        {
            dynamic model = new DynamicModel();
            model.Quarters = new[] { 0.75, 0.5, 0.25 };

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItReportsANonMatchingNumberInAnArray()
        {
            dynamic model = new DynamicModel();
            model.Quarters = new[] { 0.75, 0.5, 0.24 };

            var validator = new OneOfDoubleValidator();
            ValidationResult result = validator.Validate(model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var values = (double[]) AnonymousField(error.Values, "Allowed");
            Assert.Equal(ErrorCode.IncorrectValue, error.Code);
            Assert.Equal("Quarters", AnonymousField(error.Values, "Field"));
            Assert.Equal(0.24, AnonymousField(error.Values, "Actual"));
            Assert.Contains(0.25, values);
            Assert.Contains(0.5, values);
            Assert.Contains(0.75, values);
            Assert.Contains(1, values);
        }

        [Fact]
        public void ItIsCaseInsensitiveByDefault()
        {
            dynamic model = new DynamicModel();
            model.Rating = "GooD";

            var validator = new OneOfStringValidator();
            ValidationResult result = validator.Validate(model);

            Assert.False(result.HasErrors);
        }

        [Fact]
        public void ItHasTheOptionToBeCaseSensitive()
        {
            dynamic model = new DynamicModel();
            model.Rating = "pass";

            var validator = new OneOfCaseSensitiveStringValidator();
            ValidationResult result = validator.Validate((DynamicModel) model);

            Assert.True(result.HasErrors);
            Assert.Equal(1, result.Errors.Count);

            var error = result.Errors.First();
            var values = (string[]) AnonymousField(error.Values, "Allowed");
            Assert.Equal(ErrorCode.IncorrectValue, error.Code);
            Assert.Contains("good", values);
            Assert.Contains("okay", values);
            Assert.Contains("bad", values);
        }

        private object AnonymousField(object obj, string fieldName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(fieldName);
            return propertyInfo.GetValue(obj);
        }
    }

    class OneOfStringValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("rating", OneOf("good", "okay", "bad"));
        }
    }

    class OneOfCaseSensitiveStringValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("rating", OneOf(ValidationOptions.CaseSensitive, "good", "okay", "bad"));
        }
    }

    class OneOfDoubleValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("quarters", OneOf(0.25, 0.5, 0.75, 1.0));
        }
    }
}