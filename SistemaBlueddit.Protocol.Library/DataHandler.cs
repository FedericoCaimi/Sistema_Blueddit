using SistemaBlueddit.Domain;
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

        public static void SendResponse(TcpClient client, string serverResponse)
        {
            var stream = client.GetStream();
            var response = new Response { ServerResponse = serverResponse };
            var responseSerialized = response.SerializeObejct();
            var encodedHeader = HeaderHandler.EncodeHeader(HeaderConstants.Response, 00, responseSerialized.Length, 0);
            stream.Write(encodedHeader);
            SendData(client, responseSerialized);
        }
    }
}
