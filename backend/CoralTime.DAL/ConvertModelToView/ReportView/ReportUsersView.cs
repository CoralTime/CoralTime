using AutoMapper;
using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.Reports.Responce.DropDowns;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ReportUsersView GetViewReportUsers(this Member member, int roleId, IMapper _mapper)
        {
            var reportUsersView = _mapper.Map<Member, ReportUsersView>(member);

            reportUsersView.RoleId = roleId;

            return reportUsersView;
        }
    }
}