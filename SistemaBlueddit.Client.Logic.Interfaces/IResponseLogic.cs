using SistemaBlueddit.Domain;
using System.Net.Sockets;

namespace SistemaBlueddit.Client.Logic.Interfaces
{
    public interface IResponseLogic
    {
        Response HandleResponse(TcpClient tcpClient);
    }
}
