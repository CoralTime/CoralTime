using CoralTime.ViewModels.Clients;
using System.Collections.Generic;
using System.Text.Json;

namespace CoralTime.BL.Interfaces
{
    public interface IClientService
    {
        ClientView Create(ClientView client);

        IEnumerable<ClientView> GetAllClients();

        ClientView GetById(int clientId);

        ClientView Update(int clientId, JsonElement clientData);
    }
}