using SistemaBlueddit.Domain;
using SistemaBlueddit.Protocol.Library;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SistemaBlueddit.Client.Logic
{
    public class PostLogic
    {
        public void SendPost(TcpClient connectedClient, string option)
        {
            var post = CreatePost();
            var postSerialized = post.SerializeObejct();
            var postLength = postSerialized.Length;
            var command = Convert.ToInt16(option);
            var header = HeaderHandler.EncodeHeader(HeaderConstants.Request, command, postLength, 0);
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(header);
            DataHandler.SendData(connectedClient, postSerialized);
        }

        public Post CreatePost()
        {
            var newPost = new Post();
            var exit = false;
            var postTopics = new List<Topic>();
            Console.WriteLine("Nombre del post:");
            var name = Console.ReadLine();
            Console.WriteLine("Agregar temas. Cuando termine de agregar temas ingrese la letra s:");
            while (!exit)
            {
                Console.WriteLine("Ingrese el nombre del tema");
                var topicName = Console.ReadLine();
                if (topicName.Equals("s"))
                {
                    exit = true;
                }
                else
                {
                    if(postTopics.Exists(topic => topic.Name == topicName))
                    {
                        Console.WriteLine("Nombre del tema repetido");
                    }
                    else
                    {
                        var topic = new Topic
                        {
                            Name = topicName
                        };
                        postTopics.Add(topic);
                    }
                }
            };
            newPost.Name = name;
            newPost.Topics = postTopics;
            newPost.CreationDate = DateTime.Now;
            return newPost;
        }

        public void ExistsPost(TcpClient connectedClient, string option, string name){
            var postNameLength = name.Length;
            var command = Convert.ToInt16(option);
            var header = HeaderHandler.EncodeHeader(HeaderConstants.Request, command, postNameLength, 0);
            var connectionStream = connectedClient.GetStream();
            connectionStream.Write(header);
            DataHandler.SendData(connectedClient, name);
        }

        public void DeletePost(TcpClient tcpClient, string option, string postName)
        {
            var newPost = new Post 
            {
                Name = postName,
                Topics = new List<Topic>()
            };
            var postSerialized = newPost.SerializeObejct();
            var postLength = postSerialized.Length;
            var command = Convert.ToInt16(option);
            var header = HeaderHandler.EncodeHeader(HeaderConstants.Request, command, postLength, 0);
            var connectionStream = tcpClient.GetStream();
            connectionStream.Write(header);
            DataHandler.SendData(tcpClient, postSerialized);
        }
    }
}
