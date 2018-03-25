using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.DAL.Repositories;
using System;
using CoralTime.Common.Constants;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportsService : BaseService, IReportsService, IReportBaseService
    {
        private IReportsSettingsService _reportsSettingsService;

        public ReportsService(UnitOfWork uow, IMapper mapper, IReportsSettingsService reportsSettingsService)
            : base(uow, mapper)
        {
            _reportsSettingsService = reportsSettingsService;
        }

        public DateTime DateFrom { get; private set; }

        public DateTime DateTo { get; private set; }

        public string SingleFilteredProjectName { get; private set; } = null;

        public int SetGroupByOrDefaultGrouping(int? groupById)
        {
            return groupById ?? (int) Constants.ReportsGroupBy.Date;
        }
    }
}