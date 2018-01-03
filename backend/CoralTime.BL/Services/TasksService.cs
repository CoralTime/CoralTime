using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.ServicesInterfaces;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.BL.Services
{
    public class TasksService : _BaseService, ITasksService
    {
        public TasksService(UnitOfWork uow, IMapper mapper) 
            : base(uow, mapper) { }

        public IEnumerable<TaskType> GetAllTaskTypes()
        {
            return Uow.TaskTypeRepository.LinkedCacheGetList();
        }

        public TaskType GetById(int id)
        {
            var result = Uow.TaskTypeRepository.LinkedCacheGetList().FirstOrDefault(t => t.Id == id);

            if (result == null)
            {
                throw new CoralTimeEntityNotFoundException($"TimeEntryType with id {id} not found");
            }

            return result;
        }

        public TaskType Create(TaskView taskTypeData, string userName)
        {
            if (!IsNameUnique(taskTypeData.Name, taskTypeData.ProjectId))
            {
                throw new CoralTimeAlreadyExistsException($"Task with name {taskTypeData.Name} already exist");
            }

            var user = Uow.UserRepository.LinkedCacheGetByName(userName);

            if (taskTypeData.ProjectId != null && !user.IsAdmin)
            {
                var project = Uow.ProjectRepository.GetById(taskTypeData.ProjectId);

                if (project == null)
                {
                    throw new CoralTimeEntityNotFoundException($"Project with id {taskTypeData.ProjectId} not found");
                }

                var member = Uow.MemberRepository.LinkedCacheGetByName(userName);

                if (member == null)
                {
                    throw new CoralTimeEntityNotFoundException($"Member with userName {userName} not found");
                }

                var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();

                var memberProjectRole = Uow.MemberProjectRoleRepository.GetQueryWithIncludes()
                    .FirstOrDefault(r => r.MemberId == member.Id && r.ProjectId == project.Id && r.RoleId == managerRoleId);

                if (memberProjectRole == null)
                {
                    throw new CoralTimeForbiddenException("Forbidden");
                }
            }

            var timeEntryType = Mapper.Map<TaskView, TaskType>(taskTypeData);
            timeEntryType.IsActive = true;

            try
            {
                Uow.TaskTypeRepository.Insert(timeEntryType);
                Uow.Save();
                ClearCache();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while creating timeEntryType", e);
            }

            var result = Uow.TaskTypeRepository.GetById(timeEntryType.Id);
            return result;
        }

        public TaskType Update(dynamic taskTypeData, string userName)
        {
            var timeEntryType = Uow.TaskTypeRepository.GetById((int)taskTypeData.Id);
            if (timeEntryType == null)
            {
                throw new CoralTimeEntityNotFoundException($"timeEntryType with id {taskTypeData.Id} not found");
            }

            #region We shouldn't change projectId for Tasks

            if (timeEntryType.ProjectId != (int?)taskTypeData["projectId"])
            {
                taskTypeData["projectId"] = timeEntryType.ProjectId;
            }

            #endregion We shouldn't change projectId for Tasks

            var user = Uow.UserRepository.LinkedCacheGetByName(userName);

            if (taskTypeData["projectId"] != null && !user.IsAdmin)
            {
                var project = Uow.ProjectRepository.GetById((int)taskTypeData.ProjectId);

                if (project == null)
                {
                    throw new CoralTimeEntityNotFoundException($"Project with id {taskTypeData.ProjectId} not found");
                }

                var member = Uow.MemberRepository.LinkedCacheGetByName(userName);

                if (member == null)
                {
                    throw new CoralTimeEntityNotFoundException($"Member with userName {userName} not found");
                }

                var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();

                var memberProjectRole = Uow.MemberProjectRoleRepository.GetQueryWithIncludes()
                    .FirstOrDefault(r => r.MemberId == member.Id && r.ProjectId == project.Id && r.RoleId == managerRoleId);

                if (memberProjectRole == null)
                {
                    throw new CoralTimeForbiddenException("Forbidden");
                }
            }

            if (taskTypeData["isActive"] != null && !(bool)taskTypeData["isActive"])
            {
                var timeEntries = Uow.TimeEntryRepository
                    .GetQueryWithIncludes()
                    .Where(t => t.TaskTypesId == timeEntryType.Id && t.Date.Date == DateTime.Now.Date)
                    .ToList();

                timeEntries.ForEach(t => t.StopTimer());
            }

            UpdateService<TaskType>.UpdateObject(taskTypeData, timeEntryType);

            try
            {
                Uow.TaskTypeRepository.Update(timeEntryType);
                Uow.Save();
                ClearCache();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while updating timeEntryType", e);
            }

            var result = Uow.TaskTypeRepository.GetById(timeEntryType.Id);

            return result;
        }

        public TaskType Patch(dynamic taskTypeData, string userName)
        {
            var timeEntryType = Uow.TaskTypeRepository.GetById((int)taskTypeData.Id);
            if (timeEntryType == null)
            {
                throw new CoralTimeEntityNotFoundException($"timeEntryType with id {taskTypeData.Id} not found");
            }

            #region We shouldn't change projectId for Tasks

            if (timeEntryType.ProjectId != (int?)taskTypeData["projectId"])
            {
                taskTypeData["projectId"] = timeEntryType.ProjectId;
            }

            #endregion We shouldn't change projectId for Tasks

            var user = Uow.UserRepository.LinkedCacheGetByName(userName);

            if (taskTypeData.ProjectId != null && !user.IsAdmin)
            {
                var project = Uow.ProjectRepository.GetById((int)taskTypeData.ProjectId);

                if (project == null)
                {
                    throw new CoralTimeEntityNotFoundException($"Project with id {taskTypeData.ProjectId} not found");
                }

                var member = Uow.MemberRepository.LinkedCacheGetByName(userName);

                if (member == null)
                {
                    throw new CoralTimeEntityNotFoundException($"Member with userName {userName} not found");
                }

                var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();

                var memberProjectRole = Uow.MemberProjectRoleRepository.GetQueryWithIncludes()
                    .FirstOrDefault(r => r.MemberId == member.Id && r.ProjectId == project.Id && r.RoleId == managerRoleId);

                if (memberProjectRole == null)
                {
                    throw new CoralTimeForbiddenException("Forbidden");
                }
            }

            if (taskTypeData["isActive"] != null && !(bool)taskTypeData["isActive"])
            {
                var timeEntries = Uow.TimeEntryRepository
                    .GetQueryWithIncludes()
                    .Where(t => t.TaskTypesId == timeEntryType.Id && t.Date.Date == DateTime.Now.Date)
                    .ToList();

                timeEntries.ForEach(t => t.StopTimer());
            }

            UpdateService<TaskType>.UpdateObject(taskTypeData, timeEntryType);

            try
            {
                Uow.TaskTypeRepository.Update(timeEntryType);
                Uow.Save();
                ClearCache();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while updating timeEntryType", e);
            }

            var result = Uow.TaskTypeRepository.GetById(timeEntryType.Id);

            return result;
        }

        public bool Delete(int id)
        {
            var timeEntryType = Uow.TaskTypeRepository.GetById(id);
            if (timeEntryType == null || !timeEntryType.IsActive)
            {
                throw new CoralTimeEntityNotFoundException("timeEntryType with id " + $"{id} not found or is not active");
            }

            try
            {
                Uow.TaskTypeRepository.Delete(timeEntryType.Id);
                Uow.Save();
                ClearCache();
                return true;
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while deleting timeEntryType", e);
            }
        }

        private bool IsNameUnique(string name, int? projectId)
        {
            var task = Uow.TaskTypeRepository.GetQueryWithIncludes()
                .FirstOrDefault(t => t.IsActive
                && t.Name == name
                && (t.ProjectId == projectId || t.ProjectId == null));
            return task == null;
        }

        private void ClearCache()
        {
            Uow.TaskTypeRepository.LinkedCacheClear();
        }
    }
}