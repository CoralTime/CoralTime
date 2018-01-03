using AutoMapper;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services
{
    public class _BaseService
    {
        public _BaseService(UnitOfWork uow, IMapper mapper)
        {
            Uow = uow;
            Mapper = mapper;
        }

        protected UnitOfWork Uow { get; set; }

        protected IMapper Mapper { get; set; }
    }
}