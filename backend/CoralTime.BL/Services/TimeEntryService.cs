using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.DAL.ConvertModelToView;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.TimeEntries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoralTime.BL.Services
{
    public class TimeEntryService : BaseService, ITimeEntryService
    {
        public TimeEntryService(UnitOfWork uow, IMapper mapper)
            : base(uow, mapper) { }

        public IEnumerable<TimeEntryView> GetAllTimeEntries(DateTimeOffset dateStart, DateTimeOffset dateEnd)
        {
            var memberImpersonated = MemberImpersonated; 
            var applicationUserImpersonated = ApplicationUserImpersonated;

            var timeEntriesByMemberIdAndDates = Uow.TimeEntryRepository.GetQueryWithIncludes()
                .Where(tEntry => tEntry.MemberId == memberImpersonated.Id && dateStart <= tEntry.Date && tEntry.Date <= dateEnd)
                .ToList();

            var timeEntryViewList = new List<TimeEntryView>();

            foreach (var timeEntry in timeEntriesByMemberIdAndDates)
            {
               var timeEntryView = timeEntry.GetView(applicationUserImpersonated.UserName, Mapper);

                var isOnlyMemberAtProject = !IsAdminOrManagerOfProject(ApplicationUserCurrent.IsAdmin, memberImpersonated.Id, timeEntry.ProjectId);

                var isTimeEntryLocked = IsTimeEntryLockedByProjectSettings(timeEntry.Date, timeEntry.Project, isOnlyMemberAtProject);

                if (isTimeEntryLocked)
                {
                    timeEntryView.IsLocked = true;
                }

                timeEntryViewList.Add(timeEntryView);
            }

            return timeEntryViewList;
        }

        public TimeEntryView GetById(int id)
        {
            var timeEntryById = GetRelatedTimeEntryById(id);

            return timeEntryById.GetView(ImpersonatedUserName, Mapper);
        }

        public TimeEntryView Create(TimeEntryView timeEntryView)
        {
            var timeEntry = new TimeEntry();

            // Check if exists related entities.
            CheckRelatedEntities(timeEntryView, timeEntry, out var relatedMemberByName, out var relatedProjectById);

            // Check Lock TimeEntries: User cannot Create TimeEntry, if enable Lock TimeEntry in Project settings.  
            var isOnlyMemberAtProject = !IsAdminOrManagerOfProject(ApplicationUserCurrent.IsAdmin, relatedMemberByName.Id, timeEntry.ProjectId);
            CheckLockTimeEntryByProjectSettings(timeEntryView.Date, relatedProjectById, isOnlyMemberAtProject);

            // Check correct timing values from TimeEntryView.
            CheckCorrectTimingValues(timeEntryView.TimeValues.TimeFrom, timeEntryView.TimeValues.TimeTo, timeEntryView.TimeValues.TimeActual);

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
                return timeEntryWithUpdateRelatedEntities.GetView(ImpersonatedUserName, Mapper);
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while creating new TimeEntry", e);
            }

            #endregion
        }

        public TimeEntryView Update(TimeEntryView timeEntryView)
        {
            var timeEntryById = GetRelatedTimeEntryById(timeEntryView.Id);

            // Check if exists related entities.
            CheckRelatedEntities(timeEntryView, timeEntryById, out var relatedMemberByName, out var relatedProjectById);

            // Check Lock TimeEntries: User cannot Create TimeEntry, if enable Lock TimeEntry in Project settings.  
            var isOnlyMemberAtProject = !IsAdminOrManagerOfProject(ApplicationUserCurrent.IsAdmin, relatedMemberByName.Id, relatedProjectById.Id);
            CheckLockTimeEntryByProjectSettings(timeEntryView.Date, relatedProjectById, isOnlyMemberAtProject);

            // Check correct timing values from TimeEntryView.
            CheckCorrectTimingValues(timeEntryView.TimeValues.TimeFrom, timeEntryView.TimeValues.TimeTo, timeEntryView.TimeValues.TimeActual);

            // Check that total time per day is not grater than 24 hours.
            CheckTotalTimeAtDay(relatedMemberByName, timeEntryView, timeEntryById);

            // Update values for TimeEntry.
            UpdateValuesForTimeEntry(timeEntryById, timeEntryView);

            #region Update TimeEntry in DB.

            try
            {
                Uow.TimeEntryRepository.Update(timeEntryById);
                Uow.Save();

                return timeEntryById.GetView(ImpersonatedUserName, Mapper);
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while updating TimeEntry", e);
            }

            #endregion
        }

        public void Delete(int timeEntryId)
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
            CheckRelatedEntities(timeEntryView, timeEntryById, out var relatedMemberByName, out var relatedProjectById);

            // Check Lock TimeEntries: User cannot Create TimeEntry, if enable Lock TimeEntry in Project settings.  
            var isOnlyMemberAtProject = !IsAdminOrManagerOfProject(ApplicationUserCurrent.IsAdmin, relatedMemberByName.Id, relatedProjectById.Id);
            CheckLockTimeEntryByProjectSettings(timeEntryView.Date, relatedProjectById, isOnlyMemberAtProject);

            #region Delete TimeEntry from DB.

            try
            {
                Uow.TimeEntryRepository.Delete(timeEntryById.Id);
                Uow.Save();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while deleting the TimeEntry", e);
            }

            #endregion
        }

        #region Added Methods for Get Related Entities.

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

        private void CheckRelatedEntities(TimeEntryView timeEntryView, TimeEntry timeEntry, out Member relatedMemberByName, out Project relatedProjectById)
        {
            relatedMemberByName = MemberImpersonated;
            var isOnlyMemberAtProject = !IsAdminOrManagerOfProject(ApplicationUserCurrent.IsAdmin, MemberImpersonated.Id, timeEntry.ProjectId);

            relatedProjectById = GetRelatedProjectById(timeEntryView.ProjectId, isOnlyMemberAtProject);
            var relatedMemberById = GetRelatedMemberById(timeEntryView.MemberId, isOnlyMemberAtProject);
            var relatedTaskTypeById = GetRetaledTaskTypeById(timeEntryView.TaskTypesId, isOnlyMemberAtProject);
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

            var memberProjectRole = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .FirstOrDefault(r => r.MemberId == memberId && r.ProjectId == projectId && r.RoleId == managerRoleId);

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
                .Sum(tEntry => tEntry.TimeActual);

            var newTime = timeEntryView.TimeValues.TimeActual;
            var newDate = timeEntryView.Date;
            if (timeEntryById != null)
            {
                var oldTime = timeEntryById.TimeActual;
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

            if (totalTimeForDay > Constants.SecondsInThisDay)
            {
                throw new CoralTimeDangerException($"Total work time on the date {timeEntryView.Date} is greater than 24 hours");
            }
        }

        private void CheckCorrectTimingValues(int? timeFrom, int? timeTo, int timeActual)
        {
            var isNullTimeFrom = timeFrom == null;
            var isNullTimeTo = timeTo == null;

            var isPositiveTimeFrom = !isNullTimeFrom && timeFrom >= 0;
            var isPositiveTimeTo = !isNullTimeTo && timeTo >= 0;
            var isPositiveTimeActual = timeActual >= 0;

            // Available cases for set timings:

            // 1. Only actual time.
            var isSetOnlyActualTime = isNullTimeFrom && isNullTimeTo && isPositiveTimeActual;

            // 2. All at once properties.
            var isSetTimeFromTimeToActualTime = isPositiveTimeFrom && isPositiveTimeTo && isPositiveTimeActual;

            // 3. from To 
            if (!isNullTimeFrom && !isNullTimeTo)
            {
                var isTimeToLessThan24h = timeTo < 86400;
                var isTimeFromLessThan24h = timeFrom < 86400;
                var isTomeFromLessThanTomeTo = timeFrom <= timeTo;

                if (!isTomeFromLessThanTomeTo || !isTimeToLessThan24h || !isTimeFromLessThan24h)
                {
                    var errorMessage = "";

                    if (!isTimeToLessThan24h)
                    {
                        errorMessage += "TimeTo Less Than 24 hours.";
                    }

                    if (!isTimeFromLessThan24h)
                    {
                        errorMessage += "TimeFrom Less Than 24 hours.";
                    }

                    if (!isTomeFromLessThanTomeTo)
                    {
                        errorMessage += "TimeFrom must be Less Than Time To.";
                    }

                    throw new CoralTimeDangerException(errorMessage);
                }
            }

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

        private void CheckLockTimeEntryByProjectSettings(DateTime timeEntryDateEditing, Project projectById, bool isOnlyMemberAtProject)
        {
            if (projectById.IsTimeLockEnabled && isOnlyMemberAtProject)
            {
                var isTimeEntryLocked = IsTimeEntryLockedByProjectSettings(timeEntryDateEditing, projectById, isOnlyMemberAtProject);

                if (isTimeEntryLocked)
                {
                    throw new CoralTimeDangerException("Date for creating or updating TimeEntry is too late.");
                }
            }
        }

        private bool IsTimeEntryLockedByProjectSettings(DateTime timeEntryDateEditing, Project projectById, bool isOnlyMemberAtProject)
        {
            var isTimeEntryLocked = false;

            if (projectById.IsTimeLockEnabled && isOnlyMemberAtProject)
            {
                isTimeEntryLocked = CommonHelpers.IsTimeEntryLocked(timeEntryDateEditing, projectById.DaysBeforeStopEditTimeEntries, projectById.LockPeriod);
            }

            return isTimeEntryLocked;
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

            timeEntry.TimeActual = timeEntryView.TimeValues.TimeActual;
            timeEntry.TimeEstimated = timeEntryView.TimeValues.TimeEstimated ?? 0;
            timeEntry.TimeFrom = timeEntryView.TimeValues.TimeFrom ?? 0;
            timeEntry.TimeTo = timeEntryView.TimeValues.TimeTo ?? 0;
            timeEntry.TimeTimerStart = timeEntryView.TimeOptions.TimeTimerStart;

            timeEntry.Description = timeEntryView.Description;
            timeEntry.IsFromToShow = timeEntryView.TimeOptions.IsFromToShow;

            #endregion
        }

        #endregion
    }
}