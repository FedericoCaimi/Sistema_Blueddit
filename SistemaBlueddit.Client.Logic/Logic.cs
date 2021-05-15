using SistemaBlueddit.Client.Logic.Interfaces;
using SistemaBlueddit.Domain.Interface;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SistemaBlueddit.Client.Logic
{
    public class Logic<T> : ILogic<T> where T : ISerializable<T>
    {
        public async Task CreateAsync(TcpClient connectedClient, string option, T objectToSend)
        {
            await DataHandler<T>.SendDataAsync(connectedClient, option, HeaderConstants.Request, objectToSend);
        }

        public async Task UpdateAsync(TcpClient connectedClient, string option, T objectToSend)
        {
            await DataHandler<T>.SendDataAsync(connectedClient, option, HeaderConstants.Request, objectToSend);
        }

        public async Task ExistsAsync(TcpClient connectedClient, string option, T objectToSend)
        {
            await DataHandler<T>.SendDataAsync(connectedClient, option, HeaderConstants.Request, objectToSend);
        }

        public async Task DeleteAsync(TcpClient connectedClient, string option, T objectToSend)
        {
            await DataHandler<T>.SendDataAsync(connectedClient, option, HeaderConstants.Request, objectToSend);
        }
    }
}
