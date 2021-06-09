using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SistemaBlueddit.Server.Logic.Interfaces;
using SistemaBlueddit.Domain;

namespace SistemaBlueddit.Server
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        private ITopicLogic _topicLogic;
        public GreeterService(ILogger<GreeterService> logger, ITopicLogic topicLogic)
        {
            _logger = logger;
            _topicLogic = topicLogic; 
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var name = request.Name;
            var topic1 = new Topic
            {
                Name = name,
                Description = "descripcion"+name
            };
            _topicLogic.Add(topic1);
            Topic topic = _topicLogic.GetByName(name);
            return Task.FromResult(new HelloReply
            {
                Message = "Hello 2 " + topic.Print()
            });
        }
    }
}
