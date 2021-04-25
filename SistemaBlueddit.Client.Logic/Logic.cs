using SistemaBlueddit.Client.Logic.Interfaces;
using SistemaBlueddit.Domain.Interface;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Net.Sockets;

namespace SistemaBlueddit.Client.Logic
{
    public class Logic<T> : ILogic<T> where T : ISerializable<T>
    {
        public void Create(TcpClient connectedClient, string option, T objectToSend)
        {
            DataHandler<T>.SendData(connectedClient, option, HeaderConstants.Request, objectToSend);
        }

        public void Update(TcpClient connectedClient, string option, T objectToSend)
        {
            DataHandler<T>.SendData(connectedClient, option, HeaderConstants.Request, objectToSend);
        }

        public void Exists(TcpClient connectedClient, string option, T objectToSend)
        {
            DataHandler<T>.SendData(connectedClient, option, HeaderConstants.Request, objectToSend);
        }

        public void Delete(TcpClient connectedClient, string option, T objectToSend)
        {
            DataHandler<T>.SendData(connectedClient, option, HeaderConstants.Request, objectToSend);
        }
    }
}
