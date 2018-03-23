using AutoMapper;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.DAL.Repositories;
using System;

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

        public DateTime DateFrom { get; private set; }

        public DateTime DateTo { get; private set; }

        public string SingleFilteredProjectName { get; private set; } = null;
    }
}