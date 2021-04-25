using SistemaBlueddit.Client.Logic;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                    Console.WriteLine("* 4 - Dar de baja un tema             *");
                    Console.WriteLine("* 5 - Modificar un tema               *");
                    Console.WriteLine("* 6 - Dar de baja un post             *");
                    Console.WriteLine("* 7 - Modificar un post               *");
                    Console.WriteLine("* 99 - salir                          *");
                    Console.WriteLine("***************************************");
                    var option = Console.ReadLine();
                    Response response;
                    switch (option)
                    {
                        case "1":
                            topicLogic.SendTopic(tcpClient, option);
                            response = HandleResponse(tcpClient);
                            Console.WriteLine(response.ServerResponse);
                            break;
                        case "2":
                            postLogic.SendPost(tcpClient, option);
                            response = HandleResponse(tcpClient);
                            Console.WriteLine(response.ServerResponse);
                            break;
                        case "3":
                            postLogic.SendPost(tcpClient, option);
                            response = HandleResponse(tcpClient);
                            if (response.ServerResponse.Equals("existe"))
                            {
                                Console.WriteLine("Escriba el path completo del archivo a subir");
                                var path = Console.ReadLine();
                                fileLogic.SendFile(option, path, tcpClient);
                            }
                            else
                            {
                                Console.WriteLine("No existe el nombre de la publicacion ingresada");
                            }
                            break;
                        case "4":
                            Console.WriteLine("Escriba el nombre del tema a dar de baja");
                            var topicName = Console.ReadLine();
                            topicLogic.DeleteTopic(tcpClient, option, topicName);
                            response = HandleResponse(tcpClient);
                            Console.WriteLine(response.ServerResponse);
                            break;
                        case "5":
                            topicLogic.SendTopic(tcpClient, option);
                            response = HandleResponse(tcpClient);
                            Console.WriteLine(response.ServerResponse);
                            break;
                        case "6":
                            Console.WriteLine("Escriba el nombre del post a dar de baja");
                            var postName = Console.ReadLine();
                            postLogic.DeletePost(tcpClient, option, postName);
                            response = HandleResponse(tcpClient);
                            Console.WriteLine(response.ServerResponse);
                            break;
                        case "7":
                            postLogic.SendPost(tcpClient, option);
                            response = HandleResponse(tcpClient);
                            Console.WriteLine(response.ServerResponse);
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
            catch (Exception e)
            {
                Console.WriteLine("Ha habido un error: " + e.Message);
                Console.WriteLine("Se perdió la conexión con el servidor.");
            }
        }

        private static Response HandleResponse(TcpClient tcpClient)
        {
            var connectionStream = tcpClient.GetStream();
            var header = HeaderHandler.DecodeHeader(connectionStream);
            HeaderHandler.ValidateHeader(header, HeaderConstants.Response, 00);
            var responseData = new byte[header.DataLength];
            connectionStream.Read(responseData, 0, header.DataLength);
            var responseJson = Encoding.UTF8.GetString(responseData);
            return new Response().DeserializeObject(responseJson);
        }
    }
}