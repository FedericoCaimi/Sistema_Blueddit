using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Grpc.Net.Client;
using SistemaBlueddit.AdministrativeServer.Models;

namespace SistemaBlueddit.AdministrativeServer.Controllers
{
    [ApiController]
    [Route("topic")]
    public class TopicController : ControllerBase
    {
        public TopicController() { }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopics()
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.GetTopicsAsync( new Empty{ });
                
                //var response = reply.Json;
                var topicOut = new TopicOut{
                    Content = reply.Content,
                    Message = reply.Message
                };

                return Ok(topicOut);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
        
        [HttpGet("{name}", Name = "GetTopic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopics(string name)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.GetTopicsByNameAsync( new TopicRequest{ TopicName = name, TopicDescription = ""});
                
                //var response = reply.Json;
                var topicOut = new TopicOut{
                    Content = reply.Content,
                    Message = reply.Message
                };

                return Ok(topicOut);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] TopicIn topicIn)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.AddTopicAsync( new TopicRequest{ TopicName = topicIn.Name, TopicDescription = topicIn.Description});
                
                //var response = reply.Json;
                var topicOut = new TopicOut{
                    Content = reply.Content,
                    Message = reply.Message
                };

                return Ok(topicOut);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{name}", Name = "UpdateTopic")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upate(string name, [FromBody] TopicIn topicIn)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.UpdateTopicAsync( new TopicRequest{ TopicName = name, TopicDescription = topicIn.Description});
                
                //var response = reply.Json;
                var topicOut = new TopicOut{
                    Content = reply.Content,
                    Message = reply.Message
                };

                return Ok(topicOut);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{name}", Name = "DeleteTopic")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upate(string name)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.DeleteTopicAsync( new TopicRequest{ TopicName = name, TopicDescription = ""});
                
                //var response = reply.Json;
                var topicOut = new TopicOut{
                    Content = reply.Content,
                    Message = reply.Message
                };

                return Ok(topicOut);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
