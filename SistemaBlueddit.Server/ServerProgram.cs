using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;

namespace SistemaBlueddit.Server
{
    public class ServerProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server esta iniciando...");
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50000);
            tcpListener.Start(1);
            var acceptedClient = tcpListener.AcceptTcpClient();
            RecieveHeader(acceptedClient);
        }

        public static void RecieveHeader(TcpClient connectedClient)
        {
            var networkStream = connectedClient.GetStream();
            var header = HeaderHandler.DecodeHeader(networkStream);
            Console.WriteLine("Header recibido con exito!");
            Console.WriteLine("Header method: " + header.HeaderMethod);
            Console.WriteLine("Header command: " + header.Command);
            Console.WriteLine("Header data length: " + header.DataLength);
            Console.WriteLine("Header file name length: " + header.FileNameLength);
        }
    }
}
