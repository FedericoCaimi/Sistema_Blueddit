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

        private Posts.PostsClient _client;

        public PostController(IConfiguration configuration)
        {
            _configuration = configuration;
            _serverAddress = _configuration.GetSection("serverAddress").Value;
            using var channel = GrpcChannel.ForAddress(_serverAddress);
            _client = new Posts.PostsClient(channel);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                var reply = await _client.GetPostsAsync(new EmptyPost { });
                var response = new PostOut
                {
                    Posts = GrpcMapperHelper.ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostsByName(string name)
        {
            try
            {
                var reply = await _client.GetPostsByNameAsync(new PostRequest { Name = name });
                var response = new PostOut
                {
                    Posts = GrpcMapperHelper.ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPost([FromBody] PostIn postIn)
        {
            try
            {
                var reply  = await _client.AddPostAsync( new PostRequest{ Name = postIn.Name, Content = postIn.Content, Topics = {GrpcMapperHelper.ToGrpcTopics(postIn.Topics)}});
                
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePost([FromBody] PostIn postIn)
        {
            try
            {
                var reply  = await _client.UpdatePostAsync( new PostRequest{ Name = postIn.Name, Content = postIn.Content, Topics = { GrpcMapperHelper.ToGrpcTopics(postIn.Topics)}});
                
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                var reply  = await _client.DeletePostAsync( new PostRequest{ Name = name});
                
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
