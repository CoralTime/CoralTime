using System.Linq;
using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.MemberActions;

namespace CoralTime.BL.Services
{
    public class MemberActionService : BaseService, IMemberActionService
    {
        public MemberActionService(UnitOfWork uow, IMapper mapper = null) : base(uow, mapper)
        {
        }

        public IQueryable<MemberActionView> Get()
        {
            return Uow.MemberActionRepository.GetQuery(asNoTracking: true).Select(x=> 
                new MemberActionView
                {
                    Action = x.Action,
                    ChangedFields = x.ChangedFields,
                    ChangedObject = x.ChangedObject,
                    Date = x.Date,
                    Entity = x.Entity,
                    EntityId = x.EntityId,
                    Id = x.Id,
                    MemberFullName = x.Member.FullName,
                    MemberId = x.MemberId
                }
            );
        }
    }
}