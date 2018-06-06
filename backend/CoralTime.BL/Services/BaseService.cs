using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services
{
    public class BaseService
    {
        protected UnitOfWork Uow { get; }
        protected IMapper Mapper { get; }
        
        protected ApplicationUser BaseApplicationUserCurrent => Uow.ApplicationUserCurrent;
        protected ApplicationUser BaseApplicationUserImpersonated => Uow.ApplicationUserImpersonated;

        protected Member BaseMemberCurrent => Uow.MemberCurrent;
        protected Member BaseMemberImpersonated => Uow.MemberImpersonated;

        protected BaseService(UnitOfWork uow, IMapper mapper)
        {
            Uow = uow;
            Mapper = mapper;
        }
    }
}