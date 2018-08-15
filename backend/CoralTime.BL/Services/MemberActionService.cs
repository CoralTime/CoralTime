using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.DAL.ConvertModelToView.MemberActionView;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.MemberActions;

namespace CoralTime.BL.Services
{
    public class MemberActionService : BaseService, IMemberActionService
    {
        public MemberActionService(UnitOfWork uow, IMapper mapper = null) : base(uow, mapper)
        {
        }

        public IEnumerable<MemberActionView> Get()
        {
            return Uow.MemberActionRepository.GetQuery(asNoTracking: true).Select(x=>x.GetView(Mapper));
        }
    }
}