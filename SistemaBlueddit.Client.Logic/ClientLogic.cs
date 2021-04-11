using SistemaBlueddit.Protocol.Library;
using System;
using System.Net.Sockets;

namespace SistemaBlueddit.Client.Logic
{
    public class ClientLogic
    {
        private readonly HeaderHandler _headerHandler;
        private readonly Socket _socketClient;

        public ClientLogic(HeaderHandler headerHandler, Socket socketClient)
        {
            _headerHandler = headerHandler;
            _socketClient = socketClient;
        }

        public void SendData(short command)
        {
            if (_socketClient.Send(ConvertDataToHeader(command, new Random().Next())) == 0)
            {
                throw new SocketException();
            }
        }

        public byte[] ConvertDataToHeader(short command, int data)
        {
            return _headerHandler.EncodeHeader(command, data);
        }
    }
}
