using AutoMapper;
using CoralTime.BL.Interfaces.Reports.DDAndGrid;
using CoralTime.DAL.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace CoralTime.BL.Services.Reports.DropDownsAndGrid
{
    public partial class ReportService : _BaseService, IReportService
    {
        private IHostingEnvironment _environment { get; }

        public ReportService(UnitOfWork uow, IMapper mapper, IConfiguration configuration, IHostingEnvironment environment)
            : base(uow, mapper)
        {
            _environment = environment;
        }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}