using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Vsts;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using static CoralTime.Common.Constants.Constants;
using static CoralTime.Common.Constants.Constants.Routes;
using static CoralTime.Common.Constants.Constants.Routes.OData;


namespace CoralTime.Api.v1.Odata
{
    [Route(BaseODataControllerRoute)]
    [Authorize(Roles = ApplicationRoleAdmin)]
    public class VstsProjectIntegrationController : BaseODataController<VstsProjectIntegrationController, IVstsService>
    {
        public VstsProjectIntegrationController(IVstsService service, ILogger<VstsProjectIntegrationController> logger)
            : base(logger, service)
        {
        }

        // GET: api/v1/odata/VstsProjectIntegration
        [HttpGet]
        public IActionResult Get() => new ObjectResult(_service.Get());

        // POST api/v1/odata/VstsProjectIntegration
        [HttpPost]
        public IActionResult Create([FromBody]VstsProjectIntegrationView vstsProjectIntegrationView)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            try
            {
                var vstsProjectIntegrationViewResult = _service.Create(vstsProjectIntegrationView);
                var locationUri = $"{Request.Host}/{BaseODataRoute}/VstsProjectIntegrationView({vstsProjectIntegrationViewResult.Id})";

                return Created(locationUri, vstsProjectIntegrationViewResult);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // PUT api/v1/odata/VstsProjectIntegration(1)
        [ODataRoute(VstsProjectIntegrationWithIdRoute)]
        [HttpPut(IdRoute)]
        public IActionResult Update([FromODataUri] int id, [FromBody]VstsProjectIntegrationView vstsProjectIntegrationView)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            vstsProjectIntegrationView.Id = id;

            try
            {
                var vstsProjectIntegrationViewResult = _service.Update(vstsProjectIntegrationView);
                return new ObjectResult(vstsProjectIntegrationViewResult);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        //DELETE :api/v1/odata/VstsProjectIntegration(1)
        [ODataRoute(VstsProjectIntegrationWithIdRoute)]
        [HttpDelete(IdRoute)]
        public IActionResult Delete([FromODataUri] int id)
        {
            try
            {
                var result = _service.Delete(id);
                return new ObjectResult(null);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }
    }
}