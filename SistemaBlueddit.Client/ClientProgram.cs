using SistemaBlueddit.Client.Logic;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Net;
using System.Net.Sockets;

namespace SistemaBlueddit.Client
{
    class ClientProgram
    {
        private static readonly HeaderHandler headerHandler = new HeaderHandler();
        private static readonly Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly ClientLogic clientLogic = new ClientLogic(headerHandler, socketClient);

        static void Main(string[] args)
        {

            //var exit = false;
            //socketClient.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            //socketClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 30000));
            //Console.WriteLine("Bienvenido al client");
            //Console.WriteLine("uno - envia comando uno");
            //Console.WriteLine("dos - envia comando dos");
            //Console.WriteLine("tres - salir");
            //try
            //{
            //    while (!exit)
            //    {
            //        var option = Console.ReadLine();
            //        switch (option)
            //        {
            //            case "uno":
            //                clientLogic.SendData(1);
            //                break;
            //            case "dos":
            //                clientLogic.SendData(2);
            //                break;
            //            case "tres":
            //                clientLogic.SendData(3);
            //                exit = true;
            //                socketClient.Shutdown(SocketShutdown.Both);
            //                socketClient.Close();
            //                break;
            //            default:
            //                Console.WriteLine("Opcion invalida...");
            //                break;
            //        }
            //    }
            //}
            //catch (SocketException e)
            //{
            //    Console.WriteLine("Se perdió la conexión con el servidor: " + e.Message);
            //}
        }
    }
}