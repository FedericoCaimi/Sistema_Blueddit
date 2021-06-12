using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;

namespace SistemaBlueddit.Server
{
    public class TopicService : Topics.TopicsBase
    {
        private ITopicLogic _topicLogic;

        private IRabbitMQMessageLogic _messageLogic;

        public TopicService(ITopicLogic topicLogic, IRabbitMQMessageLogic messageLogic)
        {
            _topicLogic = topicLogic;
            _messageLogic = messageLogic;
        }

        public override Task<JsonResponse> GetTopics(GetTopicByNameRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var topics = new List<Topic>();
            if (name != null)
            {
                var topic = _topicLogic.GetByName(name);
                topics.Add(topic);
            }
            else
            {
                topics.AddRange(_topicLogic.GetAll());
            }
            return Task.FromResult(new JsonResponse
            {
                Json = JsonConvert.SerializeObject(topics)
            });
        }
    }
}
