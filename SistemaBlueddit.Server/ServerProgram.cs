using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic;
using System.Text;

namespace SistemaBlueddit.Server
{
    public class ServerProgram
    {
        private static bool _exit = false;
        public static UserLogic userLogic = new UserLogic();
        public static TopicLogic topicLogic = new TopicLogic();

        static void Main(string[] args)
        {
            Console.WriteLine("Server esta iniciando...");
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50000);
            tcpListener.Start(10);

            var threadServer = new Thread(()=> ListenForConnections(tcpListener));
            threadServer.Start();

            while (!_exit)
            {
                Console.WriteLine("Bienvenido al Servidor del Sistema Blueddit");
                Console.WriteLine("1 - Listar clientes conectados");
                Console.WriteLine("2 - Listar todos los temas del sistema");
                Console.WriteLine("99 - salir");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        userLogic.ShowUsers();
                        break;
                    case "2":
                        topicLogic.ShowTopics();
                        break;
                    case "99":
                        _exit = true;
                        tcpListener.Stop();
                        userLogic.CloseAll();
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
                    var user = new User
                    {
                        StartConnection = DateTime.Now,
                        TcpClient = acceptedClient
                    };
                    userLogic.AddUser(user);
                    Console.WriteLine("Acepte una nueva conexion");
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
                    var networkStream = acceptedClient.GetStream();
                    var header = HeaderHandler.DecodeHeader(networkStream);
                    switch (header.Command)
                    {
                        case 01:
                            var topic = topicLogic.RecieveTopic(header, networkStream);
                            Console.WriteLine("Topic: " + topic.Name + " " + topic.Description);
                            topicLogic.AddTopic(topic);
                            break;
                        default:
                            Console.WriteLine("Opcion invalida...");
                            break;
                    }
                }
                Console.WriteLine("Sali del while...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Removing client. " + e.Message);
                userLogic.RemoveUser(user);
            }
            Console.WriteLine("El cliente con hora de conexion "+ user.StartConnection.ToString()+" se desconecto");
        }
    }
}
