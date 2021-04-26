using SistemaBlueddit.Domain;
using System.Net.Sockets;

namespace SistemaBlueddit.Server.Logic.Interfaces
{
    public interface IFileLogic
    {
        BluedditFile GetFile(Header header, NetworkStream networkStream);
    }
}
