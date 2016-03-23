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

            return new UnprocessableEntityObjectResult(result);
        }

        [HttpPatch("{id}")]
        public ActionResult Patch([FromBody] Patch[] patches, int id)
        {
            dynamic model = new DynamicModel();
            model.SomeThing = 1;

            var validator = new AnyValidator();
            var result = validator.Validate(patches, model);

            return new HttpOkObjectResult(result);
        }
    }

    public class AnyValidator : Validator
    {
        protected override void ValidateModel()
        {
            Required("something");
        }

        protected override void ValidatePatch()
        {
            Allow("somETHIng");
        }
    }
}