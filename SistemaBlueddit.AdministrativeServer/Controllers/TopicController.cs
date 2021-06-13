using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
                
                return Ok(reply);
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
                
                return Ok(reply);
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
                
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{name}", Name = "UpdateTopic")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string name, [FromBody] TopicIn topicIn)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.UpdateTopicAsync( new TopicRequest{ TopicName = name, TopicDescription = topicIn.Description});
                
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{name}", Name = "DeleteTopic")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.DeleteTopicAsync( new TopicRequest{ TopicName = name, TopicDescription = ""});
                
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
