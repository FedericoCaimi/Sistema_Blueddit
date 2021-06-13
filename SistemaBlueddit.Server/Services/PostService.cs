﻿using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SistemaBlueddit.Domain;
using SistemaBlueddit.Server.Logic.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

        public override async Task<PostResponse> AddPost(PostRequest request, ServerCallContext context)
        {
            var topics = ToTopicList(request.Topics);
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
            }


            await _messageLogic.SendMessageAsync(message, "Post");

            return new PostResponse
            {
                Posts = { ToGrpcPosts(new List<Post> { post }) },
                Message = message
            };
        }

        public override async Task<PostResponse> UpdatePost(PostRequest request, ServerCallContext context)
        {
            var topics = ToTopicList(request.Topics);
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
                }
            }
            else{
                message = "No existe el post con el nombre ingresado." ;
            }


            await _messageLogic.SendMessageAsync(message, "Post");

            return new PostResponse
            {
                Posts = { ToGrpcPosts(new List<Post> { post }) },
                Message = message
            };
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

        private List<Topic> ToTopicList(RepeatedField<TopicInPost> grpcTopics)
        {
            var topics = new List<Topic>();
            foreach (var grpcTopic in grpcTopics)
            {
                var topic = new Topic
                {
                    Name = grpcTopic.Name,
                    Description = grpcTopic.Description
                };
                topics.Add(topic);
            }
            return topics;
        }
    }
}
