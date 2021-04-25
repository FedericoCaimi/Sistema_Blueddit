using SistemaBlueddit.Domain.Interface;
using System;
using System.Net;
using System.Net.Sockets;

namespace SistemaBlueddit.Domain
{
    public class User: ISerializable<User>
    {
        public TcpClient TcpClient { get; set; }
        public DateTime StartConnection { get; set; }

        public string SerializeObejct()
        {
            var userAddress = ((IPEndPoint)(TcpClient.Client.RemoteEndPoint)).Address.ToString();
            return "{User: " + userAddress + "ConnectionStartTime:" + StartConnection.ToString() + "}";
        }

        public User DeserializeObject(string objectToDeserialize)
        {
            throw new NotImplementedException();
        }

        public string Print()
        {
            var userAddress = ((IPEndPoint)(TcpClient.Client.RemoteEndPoint)).Address.ToString();
            var userPort = ((IPEndPoint)(TcpClient.Client.RemoteEndPoint)).Port.ToString();
            return "User IP: "+ userAddress + ":" + userPort + "\nHora de conexion: " +StartConnection.ToString();
        }
    }
}
