using CoralTime.DAL.Models;
using CoralTime.ViewModels.Tasks;
using System.Collections.Generic;

namespace CoralTime.BL.ServicesInterfaces
{
    public interface ITasksService
    {
        IEnumerable<TaskType> GetAllTaskTypes();

        TaskType GetById(int id);

        TaskType Create(TaskView timeEntryTypeData, string userName);

        TaskType Update(dynamic timeEntryTypeData, string userName);

        TaskType Patch(dynamic timeEntryTypeData, string userName);

        bool Delete(int id);
    }
}