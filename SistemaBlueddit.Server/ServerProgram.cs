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
        private static bool _exit = false;
        public static UserLogic userLogic = new UserLogic();
        public static TopicLogic topicLogic = new TopicLogic();
        public static PostLogic postLogic = new PostLogic();
        public static FileLogic fileLogic = new FileLogic();
        public static LocalRequestHandler localRequestHandler = new LocalRequestHandler(userLogic, topicLogic, postLogic);
        public static ClientRequestHandler clientRequestHandler = new ClientRequestHandler(topicLogic, postLogic, fileLogic);

        static void Main(string[] args)
        {
            Console.WriteLine("Server esta iniciando...");
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50000);
            tcpListener.Start(10);

            var threadServer = new Thread(()=> ListenForConnections(tcpListener));
            threadServer.Start();

            while (!_exit)
            {
                _exit = localRequestHandler.HandleLocalRequests();
            }
            tcpListener.Stop();
        }

        private static void ListenForConnections(TcpListener tcpListener)
        {
            while (!_exit)
            {
                try
                {
                    var acceptedClient = tcpListener.AcceptTcpClient();
                    var user = new User
                    {
                        StartConnection = DateTime.Now,
                        TcpClient = acceptedClient
                    };
                    userLogic.Add(user);
                    var threadClient = new Thread(() => HandleClient(user));
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

        private static void HandleClient(User user)
        {
            var acceptedClient = user.TcpClient;
            try
            {
                while (!_exit)
                {
                    clientRequestHandler.HandleClientRequests(acceptedClient);
                }
                Console.WriteLine("Sali del while...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Borrando el cliente. " + e.Message);
                userLogic.Delete(user);
            }
            Console.WriteLine("El cliente con hora de conexion " + user.StartConnection.ToString() + " se desconecto");
        }
    }
}
