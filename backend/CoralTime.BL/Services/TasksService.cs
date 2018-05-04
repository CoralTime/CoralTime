using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Exceptions;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.ConvertViewToModel;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.BL.Services
{
    public class TasksService : BaseService, ITasksService
    {
        public TasksService(UnitOfWork uow, IMapper mapper) 
            : base(uow, mapper) { }

        public IEnumerable<TaskTypeView> Get() => Uow.TaskTypeRepository.LinkedCacheGetList().Select(x=>x.GetView(Mapper));

        public TaskTypeView GetById(int id)
        {
            var taskType = Uow.TaskTypeRepository.LinkedCacheGetById(id);

            if (taskType == null)
            {
                throw new CoralTimeEntityNotFoundException($"Task with id {id} not found");
            }

            return taskType.GetView(Mapper);
        }

        public TaskTypeView Create(TaskTypeView taskTypeView)
        {
            IsTaskTypeNameHasChars(taskTypeView.Name);
            IsNameUnique(taskTypeView);

            if (taskTypeView.ProjectId != null && !ApplicationUserCurrent.IsAdmin)
            {
                if (taskTypeView.ProjectId != null)
                {
                    var project = Uow.ProjectRepository.LinkedCacheGetById((int) taskTypeView.ProjectId);
                    if (project == null)
                    {
                        throw new CoralTimeEntityNotFoundException($"Project with id {taskTypeView.ProjectId} not found");
                    }

                    var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();

                    var memberProjectRole = Uow.MemberProjectRoleRepository.LinkedCacheGetList().FirstOrDefault(r => r.MemberId == MemberCurrent.Id && r.ProjectId == project.Id && r.RoleId == managerRoleId);
                    if (memberProjectRole == null)
                    {
                        throw new CoralTimeForbiddenException("Forbidden");
                    }
                }
            }

            var taskType = taskTypeView.GetModel(Mapper, true);

            Uow.TaskTypeRepository.Insert(taskType);
            Uow.Save();
            Uow.TaskTypeRepository.LinkedCacheClear();

            return Uow.TaskTypeRepository.LinkedCacheGetById(taskType.Id).GetView(Mapper);
        }

        public TaskTypeView Update(TaskTypeView taskTypeView)
        {
            IsTaskTypeNameHasChars(taskTypeView.Name);
            IsNameUnique(taskTypeView);

            var taskType = Uow.TaskTypeRepository.GetQueryWithIncludesById(taskTypeView.Id);
            if (taskType == null)
            {
                throw new CoralTimeEntityNotFoundException($"TaskType with id {taskTypeView.Id} not found");
            }

            #region We shouldn't change projectId for Tasks

            if (taskType.ProjectId != taskTypeView.ProjectId)
            {
                taskTypeView.ProjectId = taskType.ProjectId;
            }

            #endregion We shouldn't change projectId for Tasks

            if (taskTypeView.ProjectId != null && !ApplicationUserCurrent.IsAdmin)
            {
                var project = Uow.ProjectRepository.LinkedCacheGetById((int)taskTypeView.ProjectId);
                if (project == null)
                {
                    throw new CoralTimeEntityNotFoundException($"Project with id {taskTypeView.ProjectId} not found");
                }

                var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();

                var memberProjectRole = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                    .FirstOrDefault(r => r.MemberId == MemberCurrent.Id && r.ProjectId == project.Id && r.RoleId == managerRoleId);

                if (memberProjectRole == null)
                {
                    throw new CoralTimeForbiddenException("Forbidden");
                }
            }

            if (!taskTypeView.IsActive)
            {
                var timeEntries = Uow.TimeEntryRepository.GetQueryWithIncludes()
                    .Where(t => t.TaskTypesId == taskType.Id && t.Date.Date == DateTime.Now.Date)
                    .ToList();

                timeEntries.ForEach(t => t.StopTimer());
            }

            UpdateValuesForTaskType(taskType, taskTypeView);

            Uow.TaskTypeRepository.Update(taskType);
            Uow.Save();
            Uow.TaskTypeRepository.LinkedCacheClear();

            return Uow.TaskTypeRepository.LinkedCacheGetById(taskType.Id).GetView(Mapper);
        }

        public bool Delete(int id)
        {
            var timeEntryType = Uow.TaskTypeRepository.GetById(id);
            if (timeEntryType == null || !timeEntryType.IsActive)
            {
                throw new CoralTimeEntityNotFoundException("timeEntryType with id " + $"{id} not found or is not active");
            }

            Uow.TaskTypeRepository.Delete(timeEntryType.Id);
            Uow.Save();
            Uow.TaskTypeRepository.LinkedCacheClear();

            return true;
        }

        private void UpdateValuesForTaskType(TaskType taskType, TaskTypeView taskTypeView)
        {
            #region #1. Update related entites. 

            taskType.ProjectId = taskTypeView.ProjectId;

            #endregion

            #region #2. Update other values.

            taskType.Name = taskTypeView.Name;
            taskType.IsActive = taskTypeView.IsActive;
            taskType.Color = taskTypeView.Color;
            taskType.Description = taskTypeView.Description;

            #endregion
        }

        private void IsNameUnique(TaskTypeView taskTypeView)
        {
            var isNameUnique = Uow.TaskTypeRepository.LinkedCacheGetList()
                                   .FirstOrDefault(task => task.IsActive && task.Name == taskTypeView.Name 
                                                                         && (task.ProjectId == taskTypeView.ProjectId || task.ProjectId == null)) == null;

            if (!isNameUnique)
            {
                throw new CoralTimeAlreadyExistsException($"Task with name {taskTypeView.Name} already exist");
            }
        }

        private void IsTaskTypeNameHasChars(string taskTypeViewName)
        {
            if (string.IsNullOrWhiteSpace(taskTypeViewName) && string.IsNullOrEmpty(taskTypeViewName))
            {
                throw new CoralTimeAlreadyExistsException($"Task Name cannot be null or empty or whitespace.");
            }
        }
    }
}