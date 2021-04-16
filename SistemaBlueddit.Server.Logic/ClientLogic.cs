using System;
using System.Collections.Generic;
using SistemaBlueddit.Domain;

namespace SistemaBlueddit.Server.Logic
{
    public class ClientLogic
    {
        private List<Client> ConnectedClients = new List<Client>();

        public ClientLogic()
        {

        }

        public void AddClient(Client client)
        {
            ConnectedClients.Add(client);
        }

        public void RemoveClient(Client client)
        {
            ConnectedClients.Remove(client);
        }

        public void ShowClients()
        {
            foreach (var client in ConnectedClients)
            {
                Console.WriteLine(client.PrintClient());
            }
        }
    }
}
