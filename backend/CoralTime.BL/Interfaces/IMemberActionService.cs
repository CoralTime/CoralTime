using System.Collections.Generic;
using CoralTime.ViewModels.MemberActions;

namespace CoralTime.BL.Interfaces
{
    public interface IMemberActionService
    {
        IEnumerable<MemberActionView> Get();
    }
}