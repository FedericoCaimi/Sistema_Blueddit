using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;

namespace SistemaBlueddit.Client
{
    public class ClientProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Cliente se esta iniciando");
            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 50000);
            Console.WriteLine("Conectado al server y voy a enviar el header");
            SendHeader(tcpClient);
            Console.ReadLine();
        }

        public static void SendHeader(TcpClient connectedClient)
        {
            var header = HeaderHandler.EncodeHeader(HeaderConstants.Request, 02, 5, 5);
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(header);
        }
    }
}