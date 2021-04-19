using SistemaBlueddit.Client.Logic;
using System;
using System.Net;
using System.Net.Sockets;

namespace SistemaBlueddit.Client
{
    public class ClientProgram
    {
        static void Main(string[] args)
        {
            var topicLogic = new TopicLogic();
            var postLogic = new PostLogic();
            var exit = false;

            Console.WriteLine("Cliente se esta iniciando");
            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 50000);
            Console.WriteLine("Cliente se conectó al servidor.");

            try
            {
                while (!exit)
                {
                    Console.WriteLine("Bienvenido al Sistema Blueddit");
                    Console.WriteLine("1 - Crear un nuevo tema");
                    Console.WriteLine("2 - Crear un nuevo post");
                    Console.WriteLine("99 - salir");
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            topicLogic.SendTopic(tcpClient, option);
                            break;
                        case "2":
                            postLogic.SendPost(tcpClient, option);
                            break;
                        case "99":
                            exit = true;
                            tcpClient.GetStream().Close();
                            tcpClient.Close();
                            break;
                        default:
                            Console.WriteLine("Opcion invalida...");
                            break;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Se perdió la conexión con el servidor.");
            }
        }
    }
}