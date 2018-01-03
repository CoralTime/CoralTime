using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Clients;

namespace CoralTime.DAL.ConvertersViews.ExstensionsMethods
{
    public static partial class ExstensionsMethods
    {
        public static ClientView GetView(this Client client, IMapper _mapper)
        {
            var clientView = _mapper.Map<Client, ClientView>(client);

            clientView.ProjectsCount = client.Projects?.Count ?? 0;

            return clientView;
        }

        public static ClientView GetViewActiveInactiveProjectsForClientCount(this Client client, List<Project> clientProj, IMapper _mapper)
        {
            var clientView = _mapper.Map<Client, ClientView>(client);

            if (client.IsActive)
            {
                clientView.ProjectsCount = clientProj.Count(x => x.IsActive);
            }
            else
            {
                clientView.ProjectsCount = clientProj.Count;
            }

            return clientView;
        }
    }
}