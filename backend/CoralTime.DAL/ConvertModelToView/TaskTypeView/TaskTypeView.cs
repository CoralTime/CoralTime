using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Tasks;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static TaskTypeView GetView(this TaskType taskType, IMapper _mapper) => _mapper.Map<TaskType, TaskTypeView>(taskType);
    }
}
