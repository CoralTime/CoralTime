using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services
{
    public class BaseService
    {
        protected readonly string CurrentUserName;
        protected readonly string ImpersonatedUserName;

        protected UnitOfWork Uow { get; set; }

        protected IMapper Mapper { get; set; }
        
        protected ApplicationUser ApplicationUserCurrent => Uow.ApplicationUserCurrent;
        protected ApplicationUser ApplicationUserImpersonated => Uow.ApplicationUserImpersonated;

        protected Member MemberCurrent => Uow.MemberCurrent;
        protected Member MemberImpersonated => Uow.MemberImpersonated;

        public BaseService(UnitOfWork uow, IMapper mapper)
        {
            Uow = uow;
            Mapper = mapper;

            CurrentUserName = Uow.CurrentUserName;
            ImpersonatedUserName = Uow.ImpersonatedUserName;
        }
    }
}