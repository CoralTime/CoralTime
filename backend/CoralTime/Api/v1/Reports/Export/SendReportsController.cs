using CoralTime.BL.ServicesInterfaces.Reports.Export;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
using CoralTime.ViewModels.Reports.Request.ReportsEmails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Api.v1.Reports.Export
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class SendReportsController : _BaseController<SendReportsController, IReportExportService>
    {
        public SendReportsController(IReportExportService service, ILogger<SendReportsController> logger)
            : base(logger, service) { }

        [HttpPost]
        public async Task<IActionResult> SentReport([FromBody]ReportsSendAsEmailView reportsGridData)
        {
            try
            {
                var userName = this.GetUserNameWithImpersonation();

                switch (reportsGridData.GroupById)
                {
                    case (int) ReportsGroupBy.Project:
                    {
                        await _service.SentGroupByProjects(userName, reportsGridData);
                        break;
                    }

                    case (int) ReportsGroupBy.User:
                    {
                        await _service.SentGroupByUsers(userName, reportsGridData);
                        break;
                    }

                    case (int) ReportsGroupBy.Date:
                    {
                        await _service.SentGroupByDates(userName, reportsGridData);
                        break;
                    }

                    case (int) ReportsGroupBy.Client:
                    {
                        await _service.SentGroupByClients(userName, reportsGridData);
                        break;
                    }

                    default:
                    {
                        await _service.SentGroupByNone(userName, reportsGridData);
                        break;
                    }
                }

                return Ok();
            }

            catch (Exception e)
            {
                _logger.LogWarning($"PostReportsGrid method with parameters ({reportsGridData});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }
    }
}
