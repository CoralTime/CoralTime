using System.Collections.Generic;
using System.Linq;
using CoralTime.ViewModels.MemberActions;

namespace CoralTime.BL.Interfaces
{
    public interface IMemberActionService
    {
        IQueryable<MemberActionView> Get();
    }
}