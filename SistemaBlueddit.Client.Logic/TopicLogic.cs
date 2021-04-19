using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Net.Sockets;

namespace SistemaBlueddit.Client.Logic
{
    public class TopicLogic
    {
        public TopicLogic(){}

        public void SendTopic(TcpClient connectedClient, string option)
        {
            var topic = CreateTopic();
            var topicSerialized = topic.SerializeObejct();
            var topiclength = topicSerialized.Length;
            var command = Convert.ToInt16(option);
            var header = HeaderHandler.EncodeHeader(HeaderConstants.Request, command, topiclength, 0);
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(header);
            DataHandler.SendData(connectedClient, topicSerialized);
        }

        public Topic CreateTopic()
        {
            Console.WriteLine("Nombre del tema:");
            var name = Console.ReadLine();
            Console.WriteLine("Descripcion:");
            var description = Console.ReadLine();

            var gender = new Topic
            {
                Name = name,
                Description = description
            };
            return gender;
        }
    }
}
