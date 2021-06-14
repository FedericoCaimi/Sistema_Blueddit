using Google.Protobuf.Collections;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SistemaBlueddit.AdministrativeServer.Helpers;
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
        private IConfiguration _configuration;

        private string _serverAddress;

        public PostController(IConfiguration configuration)
        {
            _configuration = configuration;
            _serverAddress = _configuration.GetSection("serverAddress").Value;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(_serverAddress);
                var client = new Posts.PostsClient(channel);
                var reply = await client.GetPostsAsync(new EmptyPost { });
                var response = new PostOut
                {
                    Posts = GrpcMapperHelper.ToDomainPost(reply.Posts),
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
                using var channel = GrpcChannel.ForAddress(_serverAddress);
                var client = new Posts.PostsClient(channel);
                var reply = await client.GetPostsByNameAsync(new PostRequest { Name = name });
                var response = new PostOut
                {
                    Posts = GrpcMapperHelper.ToDomainPost(reply.Posts),
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
                using var channel = GrpcChannel.ForAddress(_serverAddress);
                var client = new Posts.PostsClient(channel);
                var reply  = await client.AddPostAsync( new PostRequest{ Name = postIn.Name, Content = postIn.Content, Topics = {GrpcMapperHelper.ToGrpcTopics(postIn.Topics)}});
                
                var response = new PostOut
                {
                    Posts = GrpcMapperHelper.ToDomainPost(reply.Posts),
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
                using var channel = GrpcChannel.ForAddress(_serverAddress);
                var client = new Posts.PostsClient(channel);
                var reply  = await client.UpdatePostAsync( new PostRequest{ Name = postIn.Name, Content = postIn.Content, Topics = { GrpcMapperHelper.ToGrpcTopics(postIn.Topics)}});
                
                var response = new PostOut
                {
                    Posts = GrpcMapperHelper.ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                return Ok(response);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{name}", Name = "DeletePost")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(_serverAddress);
                var client = new Posts.PostsClient(channel);
                var reply  = await client.DeletePostAsync( new PostRequest{ Name = name});
                
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
