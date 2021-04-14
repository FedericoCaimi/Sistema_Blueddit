using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace SistemaBlueddit.Server
{
    public class ServerProgram
    {
        public static bool _exit = false;
        public static readonly List<TcpClient> ConnectedClients = new List<TcpClient>();
        static void Main(string[] args)
        {
            Console.WriteLine("Server esta iniciando...");
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50000);
            tcpListener.Start(10);

            var threadServer = new Thread(()=> ListenForConnections(tcpListener));
            threadServer.Start();
            
        }

        private static void ListenForConnections(TcpListener tcpListener)
        {
            while (!_exit)
            {
                try
                {
                    var acceptedClient = tcpListener.AcceptTcpClient();
                    ConnectedClients.Add(acceptedClient);
                    Console.WriteLine("Acepte una nueva conexion");
                    var threadClient = new Thread(() => HandleClient(acceptedClient));
                    threadClient.Start();

                }
                catch (SocketException se)
                {
                    Console.WriteLine("El servidor está cerrándose...");
                    _exit = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.WriteLine("Saliendo del listen...");
        }

        private static void HandleClient(TcpClient acceptedClient)
        {
            try
            {
                while (!_exit)
                {
                    RecieveHeader(acceptedClient);
                    
                }
                Console.WriteLine("Sali del while...");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Removing client....");
                ConnectedClients.Remove(acceptedClient);
            }
            Console.WriteLine("Saliendo del thread cliente....");
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
