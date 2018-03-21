using System.Linq;
using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Clients;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ClientView GetView(this Client client, IMapper _mapper)
        {
            var clientView = _mapper.Map<Client, ClientView>(client);

            clientView.ProjectsCount = client.Projects?.Count ?? 0;

            return clientView;
        }

        public static ClientView GetViewActiveInactiveProjectsForClientCount(this Client client, IMapper _mapper)
        {
            var clientView = GetView(client, _mapper);

            if (client.IsActive)
            {
                clientView.ProjectsCount = client.Projects.Count(x => x.IsActive);
            }
            else
            {
                clientView.ProjectsCount = client.Projects.Count;
            }

            return clientView;
        }
    }
}