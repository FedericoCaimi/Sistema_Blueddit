using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
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
            return Task.FromResult(new TopicsResponse
            {
                TopicName = "Hello " + request.TopicName
            });
        }
    }
}
