using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;

namespace SistemaBlueddit.Server
{
    public class TopicService : Topics.TopicsBase
    {
        private readonly ILogger<TopicService> _logger;
        private ITopicLogic _topicLogic;
        public TopicService(ILogger<TopicService> logger, ITopicLogic topicLogic)
        {
            _logger = logger;
            _topicLogic = topicLogic; 
        }

        public override Task<TopicsResponse> GetTopics(TopicsRequest request, ServerCallContext context)
        {
            /*var name = request.TopicName;
            Topic topic = _topicLogic.GetByName(name);*/
            var name = request.Topic;
            var topic1 = new Topic
            {
                Name = name,
                Description = "descripcion"+name
            };
            _topicLogic.Add(topic1);
            Topic topic = _topicLogic.GetByName(name);
            return Task.FromResult(new TopicsResponse
            {
                Message = "Hello 2 " + topic.Print()
            });
        }
    }
}
