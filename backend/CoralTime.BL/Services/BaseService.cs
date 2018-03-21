using AutoMapper;
using CoralTime.DAL.Repositories;

namespace CoralTime.BL.Services
{
    public class BaseService
    {
        protected readonly string CurrentUserName;
        protected readonly string InpersonatedUserName;

        public BaseService(UnitOfWork uow, IMapper mapper)
        {
            Uow = uow;
            Mapper = mapper;
            CurrentUserName = Uow.UserNameCurrent;
            InpersonatedUserName = Uow.UserNameImpersonated;
        }
        
        protected UnitOfWork Uow { get; set; }

        protected IMapper Mapper { get; set; }

        public void CheckCurrentUser()
        {
            CheckUserByUserName(Uow.UserNameCurrent);
        }

        public void CheckUserByUserName(string userName)
        {
            Uow.UserRepository.GetRelatedUserByName(userName);
        }
    }
}