using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportsService : BaseService, IReportsService
    {
        private IReportsSettingsService _reportsSettingsService;
        private IImageService _imageService;
        public string SingleFilteredProjectName { get; private set; } = null;

        public Member ReportMemberCurrent { get; private set; }
        public Member ReportMemberImpersonated { get; private set; }

        public ReportsService(
            UnitOfWork uow, 
            IMapper mapper, 
            IReportsSettingsService reportsSettingsService,
            IImageService imageService)
            : base(uow, mapper)
        {
            _reportsSettingsService = reportsSettingsService;
            _imageService = imageService;
            UpdateReportMembers(BaseMemberImpersonated);
        }

        private void UpdateReportMembers(Member memberFromNotification)
        {
            ReportMemberCurrent = memberFromNotification;
            ReportMemberImpersonated = memberFromNotification;
        }
    }
}