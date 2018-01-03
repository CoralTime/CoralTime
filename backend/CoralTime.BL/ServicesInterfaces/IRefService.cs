using System.Collections.Generic;
using CoralTime.DAL.Models;

namespace CoralTime.BL.ServicesInterfaces
{
    public interface IRefService
    {
        List<TaskType> GetAllTimeEntryTypes();
    }
}