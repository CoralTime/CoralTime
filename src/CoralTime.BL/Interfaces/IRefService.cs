using System.Collections.Generic;
using CoralTime.DAL.Models;

namespace CoralTime.BL.Interfaces
{
    public interface IRefService
    {
        List<TaskType> GetAllTimeEntryTypes();
    }
}