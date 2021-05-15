using SistemaBlueddit.Client.Logic.Interfaces;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SistemaBlueddit.Client.Logic
{
    public class ResponseLogic: IResponseLogic
    {
        public async Task<Response> HandleResponseAsync(TcpClient tcpClient)
        {
            var connectionStream = tcpClient.GetStream();
            var header = await HeaderHandler.DecodeHeaderAsync(connectionStream);
            HeaderHandler.ValidateHeader(header, HeaderConstants.Response, Commands.Response);
            var responseData = new byte[header.DataLength];
            await connectionStream.ReadAsync(responseData, 0, header.DataLength);
            var responseJson = Encoding.UTF8.GetString(responseData);
            return new Response().DeserializeObject(responseJson);
        }
    }
}
