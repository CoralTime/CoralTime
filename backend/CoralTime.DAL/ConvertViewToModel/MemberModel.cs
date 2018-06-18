using AutoMapper;
using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.Member;

namespace CoralTime.DAL.ConvertViewToModel
{
    public static partial class ConvertViewToModel
    {
        public static Member GetModel(this MemberView member, IMapper _mapper)
        {
            var memberModel = _mapper.Map<MemberView, Member>(member);

            return memberModel;
        }
    }
}
