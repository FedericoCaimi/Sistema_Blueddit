using SistemaBlueddit.Domain;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SistemaBlueddit.Server.Logic.Interfaces
{
    public interface IFileLogic
    {
        Task<BluedditFile> GetFileAsync(Header header, NetworkStream networkStream);
    }
}
