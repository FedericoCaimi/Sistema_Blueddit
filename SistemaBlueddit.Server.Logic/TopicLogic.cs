using SistemaBlueddit.Domain;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace SistemaBlueddit.Server.Logic
{
    public class TopicLogic
    {
        private List<Topic> _topics;

        public TopicLogic()
        {
            _topics = new List<Topic>();
        }

        public Topic RecieveTopic(Header header, NetworkStream stream)
        {
            var data = new byte[header.DataLength];
            stream.Read(data, 0, header.DataLength);
            var topicJson = Encoding.UTF8.GetString(data);
            var topic = new Topic();
            return topic.DeserializeObject(topicJson);
        }

        public void AddTopic(Topic topic)
        {
            _topics.Add(topic);
        }

        public List<Topic> Topics()
        {
            return _topics;
        }

        public void ShowTopics()
        {
            foreach (var topic in _topics)
            {
                Console.WriteLine(topic.PrintTopic());
            }
        }

        public void ValidateTopics(List<Topic> topics)
        {
            topics.ForEach((topic) =>
            {
                if(!_topics.Exists(t => t.Name == topic.Name))
                {
                    throw new Exception("Tema no existe");
                }
            });
        }
    }
}
