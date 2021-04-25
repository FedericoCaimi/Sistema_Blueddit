﻿using SistemaBlueddit.Domain.Interface;
using System;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Protocol.Library
{
    public static class DataHandler<T> where T : ISerializable<T>
    {
        public static void SendData(TcpClient connectedClient, string option, string method, T objectToSend)
        {
            var objectSerialized = objectToSend.SerializeObejct();
            var objectLength = objectSerialized.Length;
            var command = Convert.ToInt16(option);
            var header = HeaderHandler.EncodeHeader(method, command, objectLength, 0);
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(header);
            connectionStream.Write(Encoding.UTF8.GetBytes(objectSerialized));
        }
    }
}
