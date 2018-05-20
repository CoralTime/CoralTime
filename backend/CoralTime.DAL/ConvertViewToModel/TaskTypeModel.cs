using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Tasks;

namespace CoralTime.DAL.ConvertViewToModel
{
    public static partial class ConvertViewToModel
    {
        public static TaskType GetModel(this TaskTypeView member, IMapper _mapper, bool? isActive = null)
        {
            var memberModel = _mapper.Map<TaskTypeView, TaskType>(member);

            if(isActive != null)
            {
                memberModel.IsActive = (bool)isActive;
            }

            return memberModel;
        }
    }
}
