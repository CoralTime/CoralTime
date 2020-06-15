using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Repositories;
using Microsoft.Extensions.Configuration;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportsService : BaseService, IReportsService
    {
        private readonly IReportsSettingsService _reportsSettingsService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _config;
        public string SingleFilteredProjectName { get; private set; } = null;

        public Member ReportMemberCurrent { get; private set; }
        public Member ReportMemberImpersonated { get; private set; }

        public ReportsService(
            UnitOfWork uow, 
            IMapper mapper, 
            IReportsSettingsService reportsSettingsService,
            IImageService imageService,
            IConfiguration config)
            : base(uow, mapper)
        {
            _reportsSettingsService = reportsSettingsService;
            _imageService = imageService;
            _config = config;
            UpdateReportMembers(BaseMemberImpersonated);
        }

        private void UpdateReportMembers(Member memberFromNotification)
        {
            ReportMemberCurrent = memberFromNotification;
            ReportMemberImpersonated = memberFromNotification;
        }
    }
}