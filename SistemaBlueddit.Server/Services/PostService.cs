using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaBlueddit.Server.Services
{
    public class PostService: Posts.PostsBase
    {
        private ITopicLogic _topicLogic;
        private IPostLogic _postLogic;
        private IRabbitMQMessageLogic _messageLogic;

        public PostService(ITopicLogic topicLogic, IPostLogic postLogic, IRabbitMQMessageLogic messageLogic)
        {
            _topicLogic = topicLogic;
            _messageLogic = messageLogic;
            _postLogic = postLogic;
        }

        public override Task<PostResponse> GetPosts(EmptyPost request, ServerCallContext context)
        {
            var posts = new List<Post>();
            posts.AddRange(_postLogic.GetAll());
            var message = posts.Count == 0 ? "No se encontraron posts" : "";
            return Task.FromResult(new PostResponse
            {
                Posts = { ToGrpcPosts(posts) },
                Message = message
            });
        }

        public override Task<PostResponse> GetPostsByName(PostRequest request, ServerCallContext context)
        {
            var name = request.Name;
            var post = _postLogic.GetByName(name);
            var message = post == null ? $"El post {name} no existe" : "";

            return Task.FromResult(new PostResponse
            {
                Posts = { ToGrpcPosts(new List<Post> { post }) },
                Message = message
            });
        }

        private List<PostResponse.Types.Post> ToGrpcPosts(List<Post> posts)
        {
            var grpcPosts = new List<PostResponse.Types.Post>();
            foreach (var post in posts)
            {
                var grpcPost = new PostResponse.Types.Post
                {
                    Name = post.Name,
                    Content = post.Content,
                    Topics = { ToGrpcTopics(post.Topics) },
                    CreationDate = Timestamp.FromDateTime(post.CreationDate.ToUniversalTime())
                };
                grpcPosts.Add(grpcPost);
            }
            return grpcPosts;
        }

        private List<TopicInPost> ToGrpcTopics(List<Topic> topics)
        {
            var grpcTopics = new List<TopicInPost>();
            foreach (var topic in topics)
            {
                var grpcTopic = new TopicInPost
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
