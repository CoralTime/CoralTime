using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;

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