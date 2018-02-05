using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;

namespace CoralTime.DAL.ConvertersOfViewModels
{
    public static partial class ConvertersOfViewModels
    {
        public static ReportUsersView GetViewReportUsers(this Member member, int roleId, IMapper _mapper)
        {
            var reportUsersView = _mapper.Map<Member, ReportUsersView>(member);

            reportUsersView.RoleId = roleId;

            return reportUsersView;
        }
    }
}