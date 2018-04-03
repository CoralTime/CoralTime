using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportsService : BaseService, IReportsService
    {
        private IReportsSettingsService _reportsSettingsService;

        public ReportsService(UnitOfWork uow, IMapper mapper, IReportsSettingsService reportsSettingsService)
            : base(uow, mapper)
        {
            _reportsSettingsService = reportsSettingsService;
        }

        public string SingleFilteredProjectName { get; private set; } = null;
    }
}