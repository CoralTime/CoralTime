﻿using AutoMapper;
using CoralTime.BL.ServicesInterfaces;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.Common.Models;
using CoralTime.DAL.Models;
using CoralTime.DAL.ConvertersViews.ExstensionsMethods;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.TimeEntries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.BL.Services
{
    public class TimeEntryService : _BaseService, ITimeEntryService
    {
        private bool IsOnlyMemberAtProject { get; set; }

        public TimeEntryService(UnitOfWork uow, IMapper mapper)
            : base(uow, mapper) { }

        public IEnumerable<TimeEntryView> GetAllTimeEntries(string userName, DateTime dateStart, DateTime dateEnd)
        {
            var relatedMemberByName = GetRelatedMemberByUserName(userName);

            var timeEntriesByMemberIdAndDates = Uow.TimeEntryRepository.GetQueryWithIncludes()
                .Where(tEntry => tEntry.MemberId == relatedMemberByName.Id && dateStart <= tEntry.Date && tEntry.Date <= dateEnd)
                .ToList();

            return timeEntriesByMemberIdAndDates.Select(x => x.GetViewTimeEntry(userName, Mapper));
        }

        public TimeEntryView GetById(int id, string userName)
        {
            var timeEntryById = GetRelatedTimeEntryById(id);

            return timeEntryById.GetViewTimeEntry(userName, Mapper);
        }

        public TimeEntryView Create(TimeEntryView timeEntryView, string userName)
        {
            var timeEntry = new TimeEntry();

            // Check if exists related entities.
            CheckRelatedEntities(timeEntryView, timeEntry, userName, out var relatedMemberByName, out var relatedProjectById);

            // Check Lock TimeEntries: User cannot Create TimeEntry, if enable Lock TimeEntry in Project settings.  
            CheckLockTimeEntryByProjectSettings(timeEntryView.Date, relatedProjectById);

            // Check correct timing values from TimeEntryView.
            CheckCorrectTimingValues(timeEntryView.TimeFrom, timeEntryView.TimeTo, timeEntryView.Time);

            // Check that total time per day is not grater than 24 hours.
            CheckTotalTimeAtDay(relatedMemberByName, timeEntryView, timeEntry);

            // Update values for TimeEntry. 
            UpdateValuesForTimeEntry(timeEntry, timeEntryView);

            #region Insert TimeEntry to DB.

            try
            {
                Uow.TimeEntryRepository.Insert(timeEntry);
                Uow.Save();

                var timeEntryWithUpdateRelatedEntities = Uow.TimeEntryRepository.LinkedCacheGetById(timeEntry.Id);
                return timeEntryWithUpdateRelatedEntities.GetViewTimeEntry(userName, Mapper);
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while creating new TimeEntry", e);
            }

            #endregion
        }

        public TimeEntryView Update(TimeEntryView timeEntryView, string userName)
        {
            var timeEntryById = GetRelatedTimeEntryById(timeEntryView.Id);

            // Check if exists related entities.
            CheckRelatedEntities(timeEntryView, timeEntryById, userName, out var relatedMemberByName, out var relatedProjectById);

            // Check Lock TimeEntries: User cannot Create TimeEntry, if enable Lock TimeEntry in Project settings.  
            CheckLockTimeEntryByProjectSettings(timeEntryView.Date, relatedProjectById);

            // Check correct timing values from TimeEntryView.
            CheckCorrectTimingValues(timeEntryView.TimeFrom, timeEntryView.TimeTo, timeEntryView.Time);

            // Check that total time per day is not grater than 24 hours.
            CheckTotalTimeAtDay(relatedMemberByName, timeEntryView, timeEntryById);

            // Update values for TimeEntry.
            UpdateValuesForTimeEntry(timeEntryById, timeEntryView);

            #region Update TimeEntry in DB.

            try
            {
                Uow.TimeEntryRepository.Update(timeEntryById);
                Uow.Save();

                return timeEntryById.GetViewTimeEntry(userName, Mapper);
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while updating TimeEntry", e);
            }

            #endregion
        }

        public TimeEntryView Patch(TimeEntryTime timeEntryTime, string userName)
        {
            var timeEntryById = GetRelatedTimeEntryById(timeEntryTime.Id);

            var timeEntryView = new TimeEntryView
            {
                ProjectId = timeEntryById.ProjectId,
                MemberId = timeEntryById.MemberId,
                TaskTypesId = timeEntryById.TaskTypesId,
                Date = timeEntryById.Date
            };

            // Check if exists related entities.
            CheckRelatedEntities(timeEntryView, timeEntryById, userName, out var relatedMemberByName, out var relatedProjectById);

            // Check Lock TimeEntries: User cannot Create TimeEntry, if enable Lock TimeEntry in Project settings.  
            CheckLockTimeEntryByProjectSettings(timeEntryView.Date, relatedProjectById);

            // Check correct timing values from TimeEntryView.
            CheckCorrectTimingValues(timeEntryTime.TimeFrom, timeEntryTime.TimeTo, timeEntryTime.Time);

            // Update values for TimeEntryTime.
            UpdateValuesForTimeEntryTime(timeEntryById, timeEntryTime);

            #region Update and Save entity to DB.

            try
            {
                Uow.TimeEntryRepository.Update(timeEntryById);
                Uow.Save();

                return timeEntryById.GetViewTimeEntry(userName, Mapper);
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while updating TimeEntry", e);
            }

            #endregion
        }

        public bool Delete(int timeEntryId, string userName)
        {
            var timeEntryById = GetRelatedTimeEntryById(timeEntryId);

            var timeEntryView = new TimeEntryView
            {
                ProjectId = timeEntryById.ProjectId,
                MemberId = timeEntryById.MemberId,
                TaskTypesId = timeEntryById.TaskTypesId,
                Date = timeEntryById.Date
            };

            // Check if exists related entities.
            CheckRelatedEntities(timeEntryView, timeEntryById, userName, out var relatedMemberByName, out var relatedProjectById);

            // Check Lock TimeEntries: User cannot Create TimeEntry, if enable Lock TimeEntry in Project settings.  
            CheckLockTimeEntryByProjectSettings(timeEntryView.Date, relatedProjectById);

            #region Delete TimeEntry from DB.

            try
            {
                Uow.TimeEntryRepository.Delete(timeEntryById.Id);
                Uow.Save();

                return true;
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while deleting the TimeEntry", e);
            }

            #endregion
        }

        #region Added Methods for Get Related Entities.

        private ApplicationUser GetRelatedUserByName(string userName)
        {
            var relatedUserByName = Uow.UserRepository.LinkedCacheGetByName(userName);
            if (relatedUserByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"User {userName} not found.");
            }

            if (!relatedUserByName.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"User {userName} is not active.");
            }

            return relatedUserByName;
        }

        private Member GetRelatedMemberByUserName(string userName)
        {
            var relatedMemberByName = Uow.MemberRepository.LinkedCacheGetByUserName(userName);
            if (relatedMemberByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with userName {userName} not found.");
            }

            return relatedMemberByName;
        }

        private TimeEntry GetRelatedTimeEntryById(int timeEntryId)
        {
            var timeEntryById = Uow.TimeEntryRepository.LinkedCacheGetById(timeEntryId);

            if (timeEntryById == null)
            {
                throw new CoralTimeEntityNotFoundException($"TimeEntry with id {timeEntryId} not found.");
            }

            return timeEntryById;
        }

        private Project GetRelatedProjectById(int projectId, bool isOnlyMemberAtProject)
        {
            var relatedProjectById = Uow.ProjectRepository.LinkedCacheGetById(projectId);
            //var projectById = Uow.ProjectRepository.GetQuery().AsNoTracking().FirstOrDefault(x => x.Id == projectId);

            if (relatedProjectById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Project with id = {projectId} not found.");
            }

            if (!relatedProjectById.IsActive && isOnlyMemberAtProject)
            {
                throw new CoralTimeEntityNotFoundException($"Project with id = {projectId} is not active and you don't have Admin/Manager permissions at this project.");
            }

            return relatedProjectById;
        }

        private Member GetRelatedMemberById(int memberId, bool isOnlyMemberAtProject)
        {
            var relatedMemberById = Uow.MemberRepository.LinkedCacheGetById(memberId);
            if (relatedMemberById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with id = {memberId} not found.");
            }

            if (!relatedMemberById.User.IsActive && isOnlyMemberAtProject)
            {
                throw new CoralTimeForbiddenException($"Cannot create TimeEntry for member with id = {memberId} and you don't have Admin/Manager permissions at this project.");
            }

            return relatedMemberById;
        }

        private TaskType GetRetaledTaskTypeById(int taskTypeId, bool isOnlyMemberAtProject)
        {
            var relatedTaskTypeById = Uow.TaskTypeRepository.LinkedCacheGetById(taskTypeId);
            if (relatedTaskTypeById == null)
            {
                throw new CoralTimeEntityNotFoundException($"TaskType with id {taskTypeId} not found.");
            }

            if (!relatedTaskTypeById.IsActive && isOnlyMemberAtProject)
            {
                throw new CoralTimeForbiddenException($"Cannot create TimeEntry for taskType with id = {taskTypeId} is not active and you don't have Admin/Manager permissions at this project.");
            }

            return relatedTaskTypeById;
        }

        #endregion

        #region Added Methods for Checks.

        private void CheckRelatedEntities(TimeEntryView timeEntryView, TimeEntry timeEntry, string userName, out Member relatedMemberByName, out Project relatedProjectById)
        {
            var relatedUserByName = GetRelatedUserByName(userName);
            relatedMemberByName = GetRelatedMemberByUserName(userName);

            IsOnlyMemberAtProject = !IsAdminOrManagerOfProject(relatedUserByName.IsAdmin, relatedMemberByName.Id, timeEntry.ProjectId);

            relatedProjectById = GetRelatedProjectById(timeEntryView.ProjectId, IsOnlyMemberAtProject);
            var relatedMemberById = GetRelatedMemberById(timeEntryView.MemberId, IsOnlyMemberAtProject);
            var relatedTaskTypeById = GetRetaledTaskTypeById(timeEntryView.TaskTypesId, IsOnlyMemberAtProject);
        }

        private bool IsAdminOrManagerOfProject(bool userIsAdmin, int memberId, int timeEntryProjectId)
        {
            var isManagerAtThisProj = IsManagerOfProject(memberId, timeEntryProjectId);
            var isManagerOrAdminAtThisProject = userIsAdmin || isManagerAtThisProj;

            return isManagerOrAdminAtThisProject;
        }

        private bool IsManagerOfProject(int memberId, int projectId)
        {
            var managerRoleId = Uow.ProjectRoleRepository.GetManagerRoleId();

            var memberProjectRole = Uow.MemberProjectRoleRepository.LinkedCacheGetList().FirstOrDefault(r =>
                r.MemberId == memberId && r.ProjectId == projectId && r.RoleId == managerRoleId);

            if (memberProjectRole == null)
            {
                return false;
            }

            return true;
        }

        private void CheckTotalTimeAtDay(Member memberByName, TimeEntryView timeEntryView, TimeEntry timeEntryById = null)
        {
            var totalTimeForDay = Uow.TimeEntryRepository.GetQueryWithIncludes()
                .Where(tEntry => tEntry.MemberId == memberByName.Id && tEntry.Date.Date == timeEntryView.Date.Date)
                .Sum(tEntry => tEntry.Time);

            var newTime = timeEntryView.Time;
            var newDate = timeEntryView.Date;
            if (timeEntryById != null)
            {
                var oldTime = timeEntryById.Time;
                var oldDate = timeEntryById.Date;

                if (oldDate != newDate)
                {
                    totalTimeForDay = totalTimeForDay + newTime;
                }
                else if (oldTime != newTime)
                {
                    totalTimeForDay = totalTimeForDay - oldTime + newTime;
                }
            }
            else
            {
                totalTimeForDay = totalTimeForDay + newTime;
            }

            //TODO return !(timeEntry.Time/3600 >= 24) && !(Math.Abs(timeEntry.TimeTo - timeEntry.TimeFrom) >= TimeSpan.FromDays(1).TotalSeconds);
            if (totalTimeForDay > Constants.SecondsInThisDay)
            {
                throw new CoralTimeDangerException($"Total work time on the date {timeEntryView.Date} is greater than 24 hours");
            }
        }

        private void CheckCorrectTimingValues(int? timeFrom, int? timeTo, int time)
        {
            var isNullTimeFrom = timeFrom == null;
            var isNullTimeTo = timeTo == null;

            var isPositiveTimeFrom = !isNullTimeFrom && timeFrom >= 0;
            var isPositiveTimeTo = !isNullTimeTo && timeTo >= 0;
            var isPositiveTime = time >= 0;

            // Available cases for set timings:

            // 1. Only actual time.
            var isSetOnlyActualTime = isNullTimeFrom && isNullTimeTo && isPositiveTime;

            // 2. All at once properties.
            var isSetTimeFromTimeToActualTime = isPositiveTimeFrom && isPositiveTimeTo && isPositiveTime && timeFrom <= timeTo;

            if (!isSetOnlyActualTime && !isSetTimeFromTimeToActualTime)
            {
                var errorMessage = "Bad case for set: ";

                if (!isSetOnlyActualTime)
                {
                    errorMessage += ("only single Actual Time.");
                }

                if (!isSetTimeFromTimeToActualTime)
                {
                    errorMessage += "all at once properties (TimeFrom, TimeTo, Actual Time).";
                }

                throw new CoralTimeDangerException(errorMessage);
            }
        }

        private void CheckLockTimeEntryByProjectSettings(DateTime timeEntryDateEditing, Project projectById)
        {
            if (projectById.IsTimeLockEnabled && IsOnlyMemberAtProject)
            {
                var isTimeEntryLocked = CommonHelpers.IsTimeEntryLocked(timeEntryDateEditing, projectById.DaysBeforeStopEditTimeEntries, projectById.LockPeriod);

                if (isTimeEntryLocked)
                {
                    throw new CoralTimeDangerException("Date for creating or updating TimeEntry is too late.");
                }
            }
        }

        #endregion

        #region Added Methods for update values in Create/Update methods.

        private static void UpdateValuesForTimeEntry(TimeEntry timeEntry, TimeEntryView timeEntryView)
        {
            #region #1. Update related entites. 

            timeEntry.ProjectId = timeEntryView.ProjectId;
            timeEntry.MemberId = timeEntryView.MemberId;
            timeEntry.TaskTypesId = timeEntryView.TaskTypesId;

            #endregion

            #region #2. Update other values.

            timeEntry.Date = timeEntryView.Date.Date;

            timeEntry.Time = timeEntryView.Time;
            timeEntry.PlannedTime = timeEntryView.PlannedTime ?? 0;
            timeEntry.TimeFrom = timeEntryView.TimeFrom ?? 0;
            timeEntry.TimeTo = timeEntryView.TimeTo ?? 0;
            timeEntry.TimeTimerStart = timeEntryView.TimeTimerStart;

            timeEntry.Description = timeEntryView.Description;
            timeEntry.IsFromToShow = timeEntryView.IsFromToShow;

            #endregion
        }

        private static void UpdateValuesForTimeEntryTime(TimeEntry timeEntry, TimeEntryTime timeEntryTime)
        {
            timeEntry.TimeFrom = timeEntryTime.TimeFrom ?? 0;
            timeEntry.TimeTo = timeEntryTime.TimeTo ?? 0;
            timeEntry.Time = timeEntryTime.Time;
            timeEntry.TimeTimerStart = timeEntryTime.TimeTimerStart;
        }

        #endregion
    }
}