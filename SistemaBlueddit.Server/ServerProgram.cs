using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic;

namespace SistemaBlueddit.Server
{
    public class ServerProgram
    {
        public static bool _exit = false;
        public static ClientLogic clientLogic = new ClientLogic();
        static void Main(string[] args)
        {
            //ClientLogic clientLogic = new ClientLogic();
            var exit = false;
            Console.WriteLine("Server esta iniciando...");
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50000);
            tcpListener.Start(10);

            var threadServer = new Thread(()=> ListenForConnections(tcpListener));
            threadServer.Start();

            while (!exit)
            {
                Console.WriteLine("Bienvenido al Servidor del Sistema Blueddit");
                Console.WriteLine("1 - Listar clientes conectados");
                Console.WriteLine("99 - salir");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        clientLogic.ShowClients();
                        break;
                    case "99":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Opcion invalida...");
                        break;
                }
            }
            
        }

        private static void ListenForConnections(TcpListener tcpListener)
        {
            while (!_exit)
            {
                try
                {
                    var acceptedClient = tcpListener.AcceptTcpClient();
                    var client = new Client
                    {
                        StartConnection = DateTime.Now,
                        TcpClient = acceptedClient
                    };
                    clientLogic.AddClient(client);
                    Console.WriteLine("Acepte una nueva conexion");
                    var threadClient = new Thread(() => HandleClient(client));
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

        private static void HandleClient(Client client)
        {
            TcpClient acceptedClient = client.TcpClient;
            try
            {
                while (!_exit)
                {
                    RecieveHeader(acceptedClient);
                    
                }
                Console.WriteLine("Sali del while...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Removing client....");
                clientLogic.RemoveClient(client);
            }
            Console.WriteLine("El cliente con hora de conexion "+client.StartConnection.ToString()+" se desconecto");
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
