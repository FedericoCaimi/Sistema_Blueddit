using SistemaBlueddit.Domain;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SistemaBlueddit.Client.Logic.Interfaces
{
    public interface IResponseLogic
    {
        Task<Response> HandleResponseAsync(TcpClient tcpClient);
    }
}
