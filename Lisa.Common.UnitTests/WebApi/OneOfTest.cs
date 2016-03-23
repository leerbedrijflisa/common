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
        public void ItIgnoresANullValue()
        {
            dynamic model = new DynamicModel();
            model.Rating = null;

            var validator = new OneOfStringValidator();
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
}