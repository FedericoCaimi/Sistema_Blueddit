using SistemaBlueddit.Domain.Interface;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SistemaBlueddit.Client.Logic.Interfaces
{
    public interface ILogic<T> where T : ISerializable<T>
    {
        Task CreateAsync(TcpClient connectedClient, string option, T objectToSend);

        Task UpdateAsync(TcpClient connectedClient, string option, T objectToSend);

        Task ExistsAsync(TcpClient connectedClient, string option, T objectToSend);

        Task DeleteAsync(TcpClient connectedClient, string option, T objectToSend);
    }
}