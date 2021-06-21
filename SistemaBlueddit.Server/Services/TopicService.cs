using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Helpers;
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
                Topics = { GrpcMapperHelper.ToGrpcTopics(topics) },
                Message = message
            });
        }

        public override Task<TopicResponse> GetTopicsByName(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var topic = _topicLogic.GetByName(name);
            var message = topic == null ? $"El tema {name} no existe" : "";

            var topicsToReturn = topic != null 
                ? GrpcMapperHelper.ToGrpcTopics(new List<Topic> { topic }) 
                : GrpcMapperHelper.ToGrpcTopics(new List<Topic>());

            return Task.FromResult(new TopicResponse
            {
                Topics = { topicsToReturn },
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

            var message = "El tema se creo con exito";

            if (_topicLogic.Validate(topic))
            {
                _topicLogic.Add(topic);
            }
            else
            {
                message = "El tema ya existe";
                topic = null;
            }

            await _messageLogic.SendMessageAsync(message, "Topic");

            var topicsToReturn = topic != null
                ? GrpcMapperHelper.ToGrpcTopics(new List<Topic> { topic })
                : GrpcMapperHelper.ToGrpcTopics(new List<Topic>());

            return new TopicResponse
            {
                Topics = { topicsToReturn },
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

            if(message.Contains("no existe"))
            {
                topic = null;
            }

            await _messageLogic.SendMessageAsync(message, "Topic");

            var topicsToReturn = topic != null
                ? GrpcMapperHelper.ToGrpcTopics(new List<Topic> { topic })
                : GrpcMapperHelper.ToGrpcTopics(new List<Topic>());

            return new TopicResponse
            {
                Topics = { topicsToReturn },
                Message = message
            };
        }
        
        public override async Task<TopicResponse> DeleteTopic(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var message = "";

            var topic = new Topic
            {
                Name = name,
                Description = ""
            };

            var existingTopic = _topicLogic.GetByName(name);
            if (existingTopic != null)
            {
                if (_postLogic.IsTopicInPost(existingTopic))
                {
                    message = "Error. No se puede borrar el tema porque esta asociado a un post.";
                    topic = null;
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
                topic = null;
            }
            await _messageLogic.SendMessageAsync(message, "Topic");

            var topicsToReturn = topic != null
                ? GrpcMapperHelper.ToGrpcTopics(new List<Topic> { topic })
                : GrpcMapperHelper.ToGrpcTopics(new List<Topic>());

            return new TopicResponse
            {
                Topics = { topicsToReturn },
                Message = message
            };
        }
    }
}
