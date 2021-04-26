using System.Net.Sockets;

namespace SistemaBlueddit.Client.Logic.Interfaces
{
    public interface IFileLogic
    {
        void SendFile(string option, string path, TcpClient connectedClient);
    }
}
