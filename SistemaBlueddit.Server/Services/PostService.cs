using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SistemaBlueddit.Server.Helpers;

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
                Posts = { GrpcMapperHelper.ToGrpcPosts(posts) },
                Message = message
            });
        }

        public override Task<PostResponse> GetPostsByName(PostRequest request, ServerCallContext context)
        {
            var name = request.Name;
            var post = _postLogic.GetByName(name);
            var message = post == null ? $"El post {name} no existe" : "";

            var postsToReturn = post != null
                ? GrpcMapperHelper.ToGrpcPosts(new List<Post> { post })
                : GrpcMapperHelper.ToGrpcPosts(new List<Post>());

            return Task.FromResult(new PostResponse
            {
                Posts = { postsToReturn },
                Message = message
            });
        }

        public override async Task<PostResponse> AddPost(PostRequest request, ServerCallContext context)
        {
            try{
                var topics = GrpcMapperHelper.ToTopicList(request.Topics);
                var post = new Post{
                    Name = request.Name,
                    Content = request.Content,
                    Topics = topics,
                    CreationDate = DateTime.Now
                };
                var message = "";

                if (_postLogic.Validate(post) && _topicLogic.ValidateTopics(topics)){
                    _postLogic.Add(post);
                    message = $"El post {post.Name} se ha creado con exito";
                }
                else{
                    message = "Error. El post ya existe o los temas no son validos";
                    post = null;
                }


                await _messageLogic.SendMessageAsync(message, "Post");

                var postsToReturn = post != null
                    ? GrpcMapperHelper.ToGrpcPosts(new List<Post> { post })
                    : GrpcMapperHelper.ToGrpcPosts(new List<Post>());

                return new PostResponse
                {
                    Posts = { postsToReturn },
                    Message = message
                };
            }catch(Exception e)
            {
                Console.WriteLine($"No se pudo agregar el post: {e.Message}");
                return new PostResponse
                {
                    Posts = {},
                    Message = $"No se pudo agregar el post: {e.Message}"
                };
            }
        }

        public override async Task<PostResponse> UpdatePost(PostRequest request, ServerCallContext context)
        {
            var topics = GrpcMapperHelper.ToTopicList(request.Topics);
            var post = new Post{
                Name = request.Name,
                Content = request.Content,
                Topics = topics
            };
            var message = "";
            var modifyPost = _postLogic.GetByName(post.Name);
            if (modifyPost != null)
            {
                if (_topicLogic.ValidateTopics(topics)){

                    message = _postLogic.ModifyPost(post);
                }
                else{
                    message = "los temas no son validos." ;
                    post = null;
                }
            }
            else{
                message = "No existe el post con el nombre ingresado." ;
                post = null;
            }


            await _messageLogic.SendMessageAsync(message, "Post");

            var postsToReturn = post != null
                ? GrpcMapperHelper.ToGrpcPosts(new List<Post> { post })
                : GrpcMapperHelper.ToGrpcPosts(new List<Post>());

            return new PostResponse
            {
                Posts = { postsToReturn },
                Message = message
            };
        }

        public override async Task<PostResponse> DeletePost(PostRequest request, ServerCallContext context)
        {
            var name = request.Name;
            var message = "";

            var post = new Post
            {
                Name = request.Name,
                Content = "",
                Topics = new List<Topic>()
            };

            var existingPost = _postLogic.GetByName(name);
            if (existingPost != null)
            {
                _postLogic.Delete(existingPost);
                message = $"El post {name} se ha borrado con exito.";
            }
            else
            {
                message = $"No existe el post con el nombre {name}.";
                post = null; 
            }
            await _messageLogic.SendMessageAsync(message, "Post");

            var postsToReturn = post != null
                ? GrpcMapperHelper.ToGrpcPosts(new List<Post> { post })
                : GrpcMapperHelper.ToGrpcPosts(new List<Post>());

            return new PostResponse
            {
                Posts = { postsToReturn },
                Message = message
            };
        }
    }
}
