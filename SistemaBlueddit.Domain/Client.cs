using System.Runtime.ConstrainedExecution;
using System;
using System.Net.Sockets;

namespace SistemaBlueddit.Domain
{
    public class Client
    {
        public TcpClient TcpClient { get; set; }
        public DateTime StartConnection { get; set; }

        public string PrintClient()
        {
            return "Client: "+TcpClient.ToString()+" hora: "+StartConnection.ToString();
        }
    }
}
