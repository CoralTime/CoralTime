using AutoMapper;
using CoralTime.DAL.Models;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ViewModels.Tasks.TaskTypeView GetView(this TaskType taskType, IMapper _mapper) => _mapper.Map<TaskType, ViewModels.Tasks.TaskTypeView>(taskType);
    }
}
