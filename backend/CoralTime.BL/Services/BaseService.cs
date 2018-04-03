using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services
{
    public class BaseService
    {
        //TODO delete this string and use from UnitOfWork.cs finished entityes CurrentMember, ImpersonatedMember, ImpersonatedApplicationUser, CurrentApplicationUser
        protected readonly string CurrentUserName;
        protected readonly string ImpersonatedUserName;

        public ApplicationUser ApplicationUserCurrent { get; }
        public ApplicationUser ApplicationUserImpersonated { get; }

        public Member MemberCurrent { get; }
        public Member MemberImpersonated { get; }

        public BaseService(UnitOfWork uow, IMapper mapper)
        {
            Uow = uow;
            Mapper = mapper;

            CurrentUserName = Uow.CurrentUserName;
            ImpersonatedUserName = Uow.ImpersonatedUserName;

            ApplicationUserCurrent = Uow.GetUserCurrent();
            ApplicationUserImpersonated = Uow.GetUserImpersonated();

            MemberCurrent = Uow.GetMemberCurrent();
            MemberImpersonated = Uow.GetMemberImpersonated();
        }
        
        protected UnitOfWork Uow { get; set; }

        protected IMapper Mapper { get; set; }
    }
}