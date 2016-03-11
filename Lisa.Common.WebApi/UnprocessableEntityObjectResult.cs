using Microsoft.AspNet.Mvc;

namespace Lisa.Common.WebApi
{
    public class UnprocessableEntityObjectResult : ObjectResult
    {
        public UnprocessableEntityObjectResult(object value) : base(value)
        {
            StatusCode = 422;
        }
    }
}