using SistemaBlueddit.Protocol.Library;
using System;
using System.Net.Sockets;

namespace SistemaBlueddit.Client.Logic
{
    public class ClientLogic
    {
        public ClientLogic(){}

        public void SendData(short command)
        {

        }

        public byte[] ConvertDataToHeader(string headerMethod, short command, int dataLength, int fileNameLength)
        {
            return HeaderHandler.EncodeHeader(headerMethod, command, dataLength, fileNameLength);
        }
    }
}
