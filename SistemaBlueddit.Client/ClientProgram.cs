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
            var exit = false;

            Console.WriteLine("Cliente se esta iniciando");
            var tcpClient = new TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 50000);

            try
            {
                while (!exit)
                {
                    Console.WriteLine("Bienvenido al Sistema Blueddit");
                    Console.WriteLine("uno - envia comando uno");
                    Console.WriteLine("dos - salir");
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "uno":
                            Console.WriteLine("Conectado al server y voy a enviar el header");
                            SendHeader(tcpClient);
                            break;
                        case "dos":
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
            catch (SocketException e)
            {
                Console.WriteLine("Se perdió la conexión con el servidor: " + e.Message);
            }
        }

        public static void SendHeader(TcpClient connectedClient)
        {
            var header = HeaderHandler.EncodeHeader(HeaderConstants.Request, 02, 5, 5);
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(header);
        }
    }
}