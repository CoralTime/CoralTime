using System.Linq;
using AutoMapper;
using CoralTime.DAL.Models;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        public static ViewModels.Clients.ClientView GetView(this Client client, IMapper mapper)
        {
            var clientView = mapper.Map<Client, ViewModels.Clients.ClientView>(client);

            clientView.ProjectsCount = client.Projects?.Count ?? 0;

            return clientView;
        }

        public static ViewModels.Clients.ClientView GetViewActiveInactiveProjectsForClientCount(this Client client, IMapper _mapper)
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