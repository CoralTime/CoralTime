using CoralTime.DAL.Models;
using CoralTime.ViewModels.Tasks;
using System.Collections.Generic;

namespace CoralTime.BL.Interfaces
{
    public interface ITasksService
    {
        IEnumerable<TaskType> GetAllTaskTypes();

        TaskType GetById(int id);

        TaskType Create(TaskView timeEntryTypeData);

        TaskType Update(dynamic timeEntryTypeData);

        TaskType Patch(dynamic timeEntryTypeData);

        bool Delete(int id);
    }
}