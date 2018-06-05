using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services
{
    public class BaseService
    {
        protected UnitOfWork Uow { get; }
        protected IMapper Mapper { get; }
        
        protected ApplicationUser BaseApplicationUserCurrent => Uow.MemberCurrent.User;
        protected ApplicationUser BaseApplicationUserImpersonated => Uow.MemberImpersonated.User;

        protected Member BaseMemberCurrent => Uow.MemberCurrent;
        protected Member BaseMemberImpersonated => Uow.MemberImpersonated;

        protected BaseService(UnitOfWork uow, IMapper mapper = null)
        {
            Uow = uow;
            Mapper = mapper;
        }
    }
}