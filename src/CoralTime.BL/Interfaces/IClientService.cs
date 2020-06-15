using CoralTime.ViewModels.Clients;
using System.Collections.Generic;

namespace CoralTime.BL.Interfaces
{
    public interface IClientService
    {
        ClientView Create(ClientView client);

        IEnumerable<ClientView> GetAllClients();

        ClientView GetById(int clientId);

        ClientView Update(dynamic clientData);
    }
}