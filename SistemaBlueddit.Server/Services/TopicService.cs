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

        public override Task<JsonResponse> GetTopics(Empty request, ServerCallContext context)
        {
            var topics = new List<Topic>();
            topics.AddRange(_topicLogic.GetAll());
            var message = "";
            if(topics.Count == 0){
                message = "No se encontraron temas";
            }
            return Task.FromResult(new JsonResponse
            {
                Content = JsonConvert.SerializeObject(topics),
                Message = message
            });
        }

        public override Task<JsonResponse> GetTopicsByName(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var topic = _topicLogic.GetByName(name);
            var message = "";
            var content = "";
            if(topic == null){
                content = "";
                message = $"El tema {name} no existe";
            }
            else{
                content = JsonConvert.SerializeObject(topic);
                message = "";
            }
            return Task.FromResult(new JsonResponse
            {
                Content = content,
                Message = message
            });
        }

        public override Task<JsonResponse> AddTopic(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var description = request.TopicDescription;

            var topic = new Topic{
                Name = name,
                Description = description
            };

            _topicLogic.Add(topic);
            var message = "El tema se creo con exito";
            var content = JsonConvert.SerializeObject(topic);

            return Task.FromResult(new JsonResponse
            {
                Content = content,
                Message = message
            });
        }

        public override Task<JsonResponse> UpdateTopic(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var description = request.TopicDescription;

            var topic = new Topic{
                Name = name,
                Description = description
            };
            var message = _topicLogic.ModifyTopic(topic);
            var content = JsonConvert.SerializeObject(topic);

            return Task.FromResult(new JsonResponse
            {
                Content = content,
                Message = message
            });
        }
        
        public override Task<JsonResponse> DeleteTopic(TopicRequest request, ServerCallContext context)
        {
            var name = request.TopicName;
            var message = "";
            var content = "";

            var existingTopic = _topicLogic.GetByName(name);
            if (existingTopic != null)
            {
                if (_postLogic.IsTopicInPost(existingTopic))
                {
                    message = "Error. No se puede borrar el tema porque esta asociado a un post.";
                    content = "";
                }
                else
                {
                    _topicLogic.Delete(existingTopic);
                    message = $"El tema {existingTopic.Name} se ha borrado con exito.";
                    content = "";
                }
            }
            else
            {
                 message = "No existe el tema con el nombre ingresado.";
                 content = "";
            }

            return Task.FromResult(new JsonResponse
            {
                Content = content,
                Message = message
            });
        }
    }
}
