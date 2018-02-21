using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.DAL;
using CoralTime.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CoralTime.BL.Services
{
    public class RefreshDataBaseService :  BaseService, IRefreshDataBaseService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public RefreshDataBaseService(UnitOfWork uow, IMapper mapper, IServiceProvider serviceProvider, IConfiguration configuration)
            : base(uow, mapper)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task RefreshDataBase()
        {
            // TODO remove table or clear from DB!!!
            await AppDbContext.InitializeDataBase(_serviceProvider, _configuration);
        }
    }
}
