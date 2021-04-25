using SistemaBlueddit.Domain.Interface;
using System.Net.Sockets;

namespace SistemaBlueddit.Client.Logic.Interfaces
{
    public interface ILogic<T> where T : ISerializable<T>
    {
        void Create(TcpClient connectedClient, string option, T objectToSend);

        void Update(TcpClient connectedClient, string option, T objectToSend);

        void Exists(TcpClient connectedClient, string option, T objectToSend);

        void Delete(TcpClient connectedClient, string option, T objectToSend);
    }
}