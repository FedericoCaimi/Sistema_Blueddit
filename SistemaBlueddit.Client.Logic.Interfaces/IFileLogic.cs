using System.Net.Sockets;
using System.Threading.Tasks;

namespace SistemaBlueddit.Client.Logic.Interfaces
{
    public interface IFileLogic
    {
        Task SendFileAsync(string option, string path, TcpClient connectedClient);
    }
}
