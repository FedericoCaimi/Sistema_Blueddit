using System;
using System.Net.Sockets;
using System.Text;
using SistemaBlueddit.Protocol.Library;

namespace SistemaBlueddit.Client.Logic
{
    public class GenderLogic
    {
        public GenderLogic(){}

        public void AddGender(TcpClient connectedClient, string option)
        {
            Console.WriteLine("Nombre del genero:");
            var name = Console.ReadLine();
            Console.WriteLine("Descripcion:");
            var description = Console.ReadLine(); 

            var gender = "{\"Name\":\""+name+"\",\"Description\":\""+description+"\"}";
            var genderlength = gender.Length;
            var command = Convert.ToInt16(option);
            SendHeader(connectedClient, HeaderConstants.Request, command, genderlength, 0);
            SendData(connectedClient, gender);


        }

        private void SendHeader(TcpClient connectedClient, string headerMethod, short command, int dataLength, int fileNameLength)
        {
            var header = HeaderHandler.EncodeHeader(headerMethod, command, dataLength, fileNameLength);
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(header);
        }

        private void SendData(TcpClient connectedClient, string data)
        {
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(Encoding.UTF8.GetBytes(data));
        }
    }
}
