//using CoralTime.BL.ServicesInterfaces;
//using System;
//using System.Collections.Generic;
//using CoralTime.DAL.Models;
//using CoralTime.Common.Exceptions;
//using CoralTime.BL.Helpers;
//using CoralTime.Common.Helpers;
//using CoralTime.DAL.Repositories;
//using CoralTime.ViewModels.Settings;
//using AutoMapper;

//namespace CoralTime.BL.Services
//{
//    public class SettingsService : _BaseService, ISettingsService
//    {
//        public SettingsService(UnitOfWork uow, IMapper mapper) 
//            : base(uow, mapper) { }

//        public Setting Create(SettingsView setting)
//        {
//           bool isNameUnique = Uow.SettingsRepository.GetById(setting.Name) == null;

//            var newSetting = Mapper.Map<SettingsView, Setting>(setting);
                
//            BLHelpers.CheckSettingsErrors(newSetting, isNameUnique);

//            try
//            {
//                Uow.SettingsRepository.Insert(newSetting);
//                Uow.Save();
//                Uow.SettingsRepository.ClearEntityCache();
//            }
//            catch (Exception e)
//            {
//                throw new CoralTimeDangerException("An error occurred while creating setting", e);
//            }

//            var result = Uow.SettingsRepository.GetById(newSetting.Name);
//            return result;
//        }

//        public bool Delete(string name)
//        {
//            var setting = Uow.SettingsRepository.GetById(name);
//            if (setting == null)
//            {
//                throw new CoralTimeEntityNotFoundException("Setting with name " + name + " not found");
//            }

//            try
//            {
//                Uow.SettingsRepository.Delete(setting.Name);
//                Uow.Save();
//                Uow.SettingsRepository.ClearEntityCache();
//                return true;
//            }
//            catch (Exception e)
//            {
//                throw new CoralTimeDangerException("An error occurred while deleting the setting", e);
//            }

//        }

//        public IEnumerable<Setting> GetAllSettings()
//        {
//            return Uow.SettingsRepository.LinkedCacheGetList();
//        }

//        public Setting GetByName(string name)
//        {
//            var result = Uow.SettingsRepository.GetById(name);
//            if (result == null)
//            {
//                throw new CoralTimeEntityNotFoundException("Setting with name " + name + " not found");
//            }

//            return result;
//        }

//        public Setting Patch(dynamic newSetting)
//        {
//            var setting = Uow.SettingsRepository.GetById((string)newSetting["name"]);
//            if (setting == null)
//            {
//                throw new CoralTimeEntityNotFoundException("Setting with name " + newSetting["name"] + " not found");
//            }

//            UpdateService<Setting>.UpdateObject(newSetting, setting);

//            BLHelpers.CheckSettingsErrors(setting);

//            try
//            {
//                Uow.SettingsRepository.Update(setting);
//                Uow.Save();
//                Uow.SettingsRepository.ClearEntityCache();
//            }
//            catch (Exception e)
//            {
//                throw new CoralTimeDangerException("An error occurred while updating setting", e);
//            }

//            var result = Uow.SettingsRepository.GetById(setting.Name);

//            return result;
//        }

//        public Setting Update(dynamic newSetting)
//        {
//            var setting = Uow.SettingsRepository.GetById((string)newSetting["name"]);
//            if (setting == null)
//            {
//                throw new CoralTimeEntityNotFoundException("Setting with name " + newSetting["name"] + " not found");
//            }
                        
//            UpdateService<Setting>.UpdateObject(newSetting, setting);

//            BLHelpers.CheckSettingsErrors(setting);

//            try
//            {
//                Uow.SettingsRepository.Update(setting);
//                Uow.Save();
//                Uow.SettingsRepository.ClearEntityCache();
//            }
//            catch (Exception e)
//            {
//                throw new CoralTimeDangerException("An error occurred while updating setting", e);
//            }

//            var result = Uow.SettingsRepository.GetById(setting.Name);
            
//            return result;
//        }
//    }
//}
