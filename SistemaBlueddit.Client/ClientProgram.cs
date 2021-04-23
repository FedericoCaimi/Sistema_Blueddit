using SistemaBlueddit.Client.Logic;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SistemaBlueddit.Client
{
    public class ClientProgram
    {
        public static bool exit = false;

        static void Main(string[] args)
        {
            var topicLogic = new TopicLogic();
            var postLogic = new PostLogic();
            var fileLogic = new FileLogic();

            Console.WriteLine("Cliente se esta iniciando");

            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 50000);

            //var handleResponseThread = new Thread(() => HandleResponse(tcpClient));
            //handleResponseThread.Start();

            Console.WriteLine("Cliente se conectó al servidor.");

            try
            {
                while (!exit)
                {
                    Console.WriteLine("***************************************");
                    Console.WriteLine("*   Bienvenido al Sistema Blueddit    *");
                    Console.WriteLine("***************************************");
                    Console.WriteLine("* 1 - Crear un nuevo tema             *");
                    Console.WriteLine("* 2 - Crear un nuevo post             *");
                    Console.WriteLine("* 3 - Subir archivo a una publicacion *");
                    Console.WriteLine("* 99 - salir                          *");
                    Console.WriteLine("***************************************");
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            topicLogic.SendTopic(tcpClient, option);
                            HandleResponse(tcpClient);
                            break;
                        case "2":
                            postLogic.SendPost(tcpClient, option);
                            HandleResponse(tcpClient);
                            break;
                        case "3":
                            //Console.WriteLine("Escriba el nombre de la publicacion");
                            //var name = Console.ReadLine();
                            //postLogic.ExistsPost(tcpClient, option, name);
                            //HandleResponse(tcpClient);
                            //if()
                            Console.WriteLine("Escriba el path completo del archivo a subir");
                            var path = Console.ReadLine();
                            fileLogic.SendFile(option, path, tcpClient);
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

        private static void HandleResponse(TcpClient tcpClient)
        {
            //while (!exit)
            //{
                var connectionStream = tcpClient.GetStream();
                var header = HeaderHandler.DecodeHeader(connectionStream);
                HeaderHandler.ValidateHeader(header, HeaderConstants.Response, 00);
                var responseData = new byte[header.DataLength];
                connectionStream.Read(responseData, 0, header.DataLength);
                var responseJson = Encoding.UTF8.GetString(responseData);
                var response = new Response().DeserializeObject(responseJson);
                Console.WriteLine(response.ServerResponse);

            //}
        }
    }
}