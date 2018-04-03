using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Clients;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using CoralTime.DAL.ConvertModelToView;

namespace CoralTime.BL.Services
{
    public class ClientService : BaseService, IClientService
    {
        public ClientService(UnitOfWork uow, IMapper mapper)
            : base(uow, mapper) { }

        public ClientView Create(ClientView clientData)
        {
            if (!IsNameUnique(clientData.Name))
            {
                throw new CoralTimeAlreadyExistsException($"Client with name {clientData.Name} already exist.");
            }

            var client = Mapper.Map<ClientView, Client>(clientData);
            client.IsActive = true;

            BLHelpers.CheckClientsErrors(client);

            try
            {
                Uow.ClientRepository.Insert(client);
                Uow.Save();

                Uow.ClientRepository.LinkedCacheClear();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while creating new client", e);
            }

            var getClient = Uow.ClientRepository.LinkedCacheGetById(client.Id);

            var result = getClient.GetView(Mapper);

            return result;
        }

        public IEnumerable<ClientView> GetAllClients()
        {
            var result = Uow.ClientRepository.LinkedCacheGetList()
                .Select(client => client.GetViewActiveInactiveProjectsForClientCount(Mapper));

            return result;
        }

        public ClientView GetById(int clientId)
        {
            var client = Uow.ClientRepository.LinkedCacheGetById(clientId);

            if (client == null)
            {
                throw new CoralTimeEntityNotFoundException($"Client with id {clientId} not found.");
            }

            var result = client.GetView(Mapper);
            return result;
        }

        public ClientView Update(dynamic clientData)
        {
            var client = Uow.ClientRepository.GetById((int)clientData.Id);

            if (client == null)
            {
                throw new CoralTimeEntityNotFoundException($"Client with id {clientData.Id} not found");
            }

            UpdateService<Client>.UpdateObject(clientData, client);

            BLHelpers.CheckClientsErrors(client);

            // Get all project linked to this client and remember projects that active before an archiving.
            RememberProjectStatusBeforeArchiving(clientData, client);

            try
            {
                Uow.ClientRepository.Update(client);
                Uow.Save();
                Uow.ClientRepository.LinkedCacheClear();
            }
            catch (Exception e)
            {
                throw new CoralTimeDangerException("An error occurred while updating the client", e);
            }

            var resultClient = Uow.ClientRepository.LinkedCacheGetById(client.Id);
            var result = resultClient.GetView(Mapper);
            return result;
        }

        #region help methods

        private void RememberProjectStatusBeforeArchiving(dynamic clientData, Client client)
        {
            if (UpdateService<Client>.HasField(clientData, "isActive"))
            {
                var isActive = (bool)clientData["isActive"];
                var clientProjects = Uow.ClientRepository.GetQueryWithIncludes()
                    .Include(c => c.Projects)
                    .FirstOrDefault(c => c.Id == client.Id)
                    .Projects;

                // Remember projects that active before an archiving and set isActive of project = false, when client set is archive.
                if (clientProjects.Any())
                {
                    clientProjects.ForEach(project =>
                    {
                        if (isActive) // If client as active status.
                        {
                            project.IsActive = project.IsActiveBeforeArchiving;
                        }
                        else // If client has inactive status.
                        {
                            project.IsActiveBeforeArchiving = project.IsActive;
                            project.IsActive = false;

                            StopTimerForProjectTimeEntries(project.Id);
                        }
                    });
                }
            }
        }

        private void StopTimerForProjectTimeEntries(int projectId)
        {
            var timeEntries = Uow.TimeEntryRepository.GetQueryWithIncludes()
                .Where(t => t.ProjectId == projectId && t.Date.Date == DateTime.Now.Date)
                .ToList();

            timeEntries.ForEach(t => t.StopTimer());
        }

        private bool IsNameUnique(string name)
        {
            return Uow.ClientRepository.LinkedCacheGetByName(name) == null;
        }

        #endregion help methods
    }
}