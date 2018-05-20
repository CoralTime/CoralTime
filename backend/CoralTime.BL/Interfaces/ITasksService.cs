using CoralTime.ViewModels.Tasks;
using System.Collections.Generic;

namespace CoralTime.BL.Interfaces
{
    public interface ITasksService
    {
        IEnumerable<TaskTypeView> Get();

        TaskTypeView GetById(int id);

        TaskTypeView Create(TaskTypeView timeEntryTypeData);

        TaskTypeView Update(TaskTypeView timeEntryTypeData);

        bool Delete(int id);
    }
}