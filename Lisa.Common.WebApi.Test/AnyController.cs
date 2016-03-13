using System;
using Microsoft.AspNet.Mvc;

namespace Lisa.Common.WebApi.Test
{
    [Route("/any/")]
    public class AnyController
    {
        [HttpGet]
        public ActionResult Get()
        {
            dynamic model = new DynamicModel();
            model.FoOo = 2;
            model.BaRr = "far";
            return new HttpOkObjectResult(model);
        }

        [HttpPost]
        public ActionResult Post([FromBody] DynamicModel model)
        {
            var result = new AnyValidator().Validate(model);

            return new UnprocessableEntityObjectResult(result.Errors);
        }
    }

    public class AnyValidator : Validator
    {
        public override void ValidateModel()
        {
            Required("something");
        }
    }
}