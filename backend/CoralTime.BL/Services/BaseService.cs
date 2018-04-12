using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services
{
    public class BaseService
    {
        protected UnitOfWork Uow { get; }
        protected IMapper Mapper { get; }
        
        protected ApplicationUser ApplicationUserCurrent => Uow.ApplicationUserCurrent;
        protected ApplicationUser ApplicationUserImpersonated => Uow.ApplicationUserImpersonated;
        protected Member MemberCurrent => Uow.MemberCurrent;
        protected Member MemberImpersonated => Uow.MemberImpersonated;
        protected string CurrentUserName => Uow.CurrentUserName;
        protected string ImpersonatedUserName => Uow.ImpersonatedUserName;

        public BaseService(UnitOfWork uow, IMapper mapper)
        {
            Uow = uow;
            Mapper = mapper;
        }
    }
}