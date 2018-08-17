using CoralTime.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CoralTime.Common.Constants.Constants;
using static CoralTime.Common.Constants.Constants.Routes.OData;


namespace CoralTime.Api.v1.Odata
{
    [Route(BaseODataControllerRoute)]
    [Authorize(Roles = ApplicationRoleAdmin)]
    public class MemberActionsController : BaseODataController<MemberActionsController, IMemberActionService>
    {
        public MemberActionsController(IMemberActionService service, ILogger<MemberActionsController> logger)
            : base(logger, service)
        {
        }

        // GET: api/v1/odata/MemberActions
        [HttpGet]
        public IActionResult Get() => Ok(_service.Get());
    }
}