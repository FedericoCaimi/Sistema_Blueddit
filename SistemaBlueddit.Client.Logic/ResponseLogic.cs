using SistemaBlueddit.Client.Logic.Interfaces;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Client.Logic
{
    public class ResponseLogic: IResponseLogic
    {
        public Response HandleResponse(TcpClient tcpClient)
        {
            var connectionStream = tcpClient.GetStream();
            var header = HeaderHandler.DecodeHeader(connectionStream);
            HeaderHandler.ValidateHeader(header, HeaderConstants.Response, Commands.Response);
            var responseData = new byte[header.DataLength];
            connectionStream.Read(responseData, 0, header.DataLength);
            var responseJson = Encoding.UTF8.GetString(responseData);
            return new Response().DeserializeObject(responseJson);
        }
    }
}
