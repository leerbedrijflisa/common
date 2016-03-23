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
            Assert.Contains(0.25, values);
            Assert.Contains(0.5, values);
            Assert.Contains(0.75, values);
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

    class OneOfDoubleValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("quarters", OneOf(0.25, 0.5, 0.75));
        }
    }
}