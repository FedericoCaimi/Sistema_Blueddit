using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Protocol.Library
{
    public static class DataHandler
    {
        public static void SendData(TcpClient connectedClient, string data)
        {
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(Encoding.UTF8.GetBytes(data));
        }
    }
}
