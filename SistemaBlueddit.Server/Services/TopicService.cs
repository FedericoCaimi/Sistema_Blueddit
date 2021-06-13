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
        private IPostLogic _postLogic;
        private IRabbitMQMessageLogic _messageLogic;

        public TopicService(ITopicLogic topicLogic, IPostLogic postLogic, IRabbitMQMessageLogic messageLogic)
        {
            _topicLogic = topicLogic;
            _messageLogic = messageLogic;
            _postLogic = postLogic;
        }

        public override Task<TopicResponse> GetTopics(Empty request, ServerCallContext context)
        {
            var topics = new List<Topic>();
            topics.AddRange(_topicLogic.GetAll());
            var message = topics.Count == 0 ? "No se encontraron temas" : "";
            return Task.FromResult(new TopicResponse
            {
                Topics = { ToGrpcTopics(topics) },
                Message = message
            });
        }

        public override Task<TopicResponse> GetTopicsByName(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var topic = _topicLogic.GetByName(name);
            var message = topic == null ? $"El tema {name} no existe" : "";

            return Task.FromResult(new TopicResponse
            {
                Topics = { ToGrpcTopics(new List<Topic>{ topic }) },
                Message = message
            });
        }

        public override async Task<TopicResponse> AddTopic(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var description = request.TopicDescription;

            var topic = new Topic{
                Name = name,
                Description = description
            };

            _topicLogic.Add(topic);
            var message = "El tema se creo con exito";

            await _messageLogic.SendMessageAsync(message, "Topic");

            return new TopicResponse
            {
                Topics = { ToGrpcTopics(new List<Topic> { topic }) },
                Message = message
            };
        }

        public override async Task<TopicResponse> UpdateTopic(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var description = request.TopicDescription;

            var topic = new Topic{
                Name = name,
                Description = description
            };
            var message = _topicLogic.ModifyTopic(topic);

            await _messageLogic.SendMessageAsync(message, "Topic");

            return new TopicResponse
            {
                Topics = { ToGrpcTopics(new List<Topic> { topic }) },
                Message = message
            };
        }
        
        public override async Task<TopicResponse> DeleteTopic(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var message = "";

            var existingTopic = _topicLogic.GetByName(name);
            if (existingTopic != null)
            {
                if (_postLogic.IsTopicInPost(existingTopic))
                {
                    message = "Error. No se puede borrar el tema porque esta asociado a un post.";
                }
                else
                {
                    _topicLogic.Delete(existingTopic);
                    message = $"El tema {existingTopic.Name} se ha borrado con exito.";
                }
            }
            else
            {
                 message = "No existe el tema con el nombre ingresado.";
            }
            await _messageLogic.SendMessageAsync(message, "Topic");
            return new TopicResponse
            {
                Message = message
            };
        }

        private List<TopicResponse.Types.Topic> ToGrpcTopics(List<Topic> topics)
        {
            var grpcTopics = new List<TopicResponse.Types.Topic>();
            foreach (var topic in topics)
            {
                var grpcTopic = new TopicResponse.Types.Topic
                {
                    Name = topic.Name,
                    Description = topic.Description
                };
                grpcTopics.Add(grpcTopic);
            }
            return grpcTopics;
        }
    }
}
