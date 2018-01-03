using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports.ReportsDropwDowns;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ReportUsersView GetViewReportUsers(this Member member, int roleId, IMapper _mapper)
        {
            var reportUsersView = _mapper.Map<Member, ReportUsersView>(member);

            reportUsersView.RoleId = roleId;

            return reportUsersView;
        }
    }
}