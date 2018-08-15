using AutoMapper;
using CoralTime.DAL.Models.LogChanges;

namespace CoralTime.DAL.ConvertModelToView.MemberActionView
{
    public static partial class ConvertModelToView
    {
        public static ViewModels.MemberActions.MemberActionView GetView(this MemberAction memberAction, IMapper mapper) => mapper.Map<MemberAction, ViewModels.MemberActions.MemberActionView>(memberAction);
    }
}