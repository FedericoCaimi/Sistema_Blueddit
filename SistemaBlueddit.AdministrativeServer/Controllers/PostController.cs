using Google.Protobuf.Collections;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaBlueddit.AdministrativeServer.Models;
using SistemaBlueddit.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaBlueddit.AdministrativeServer.Controllers
{
    [ApiController]
    [Route("post")]
    public class PostController: ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Posts.PostsClient(channel);
                var reply = await client.GetPostsAsync(new EmptyPost { });
                var response = new PostOut
                {
                    Posts = ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostsByName(string name)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Posts.PostsClient(channel);
                var reply = await client.GetPostsByNameAsync(new PostRequest { Name = name });
                var response = new PostOut
                {
                    Posts = ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPost([FromBody] PostIn postIn)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Posts.PostsClient(channel);
                var reply  = await client.AddPostAsync( new PostRequest{ Name = postIn.Name, Content = postIn.Content, Topics = {ToGrpcTopics(postIn.Topics)}});
                
                var response = new PostOut
                {
                    Posts = ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                return Ok(response);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePost([FromBody] PostIn postIn)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Posts.PostsClient(channel);
                var reply  = await client.UpdatePostAsync( new PostRequest{ Name = postIn.Name, Content = postIn.Content, Topics = {ToGrpcTopics(postIn.Topics)}});
                
                var response = new PostOut
                {
                    Posts = ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                return Ok(response);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private List<Post> ToDomainPost(RepeatedField<PostResponse.Types.Post> grpcPosts)
        {
            var posts = new List<Post>();
            foreach(var grpcPost in grpcPosts)
            {
                var post = new Post
                {
                    Name = grpcPost.Name,
                    Content = grpcPost.Content,
                    CreationDate = grpcPost.CreationDate.ToDateTime(),
                    Topics = ToDomainTopic(grpcPost.Topics)
                };
                posts.Add(post);
            }
            return posts;
        }

        private List<Topic> ToDomainTopic(RepeatedField<TopicInPost> grpcTopics)
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
