using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Grpc.Net.Client;
using SistemaBlueddit.AdministrativeServer.Models;
using Microsoft.Extensions.Configuration;

namespace SistemaBlueddit.AdministrativeServer.Controllers
{
    [ApiController]
    [Route("topic")]
    public class TopicController : ControllerBase
    {
        private IConfiguration _configuration;

        private string _serverAddress;

        private Topics.TopicsClient _client;

        public TopicController(IConfiguration configuration) 
        {
            _configuration = configuration;
            _serverAddress = _configuration.GetSection("serverAddress").Value;
            using var channel = GrpcChannel.ForAddress(_serverAddress);
            _client = new Topics.TopicsClient(channel);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopics()
        {
            try
            {
                var reply  = await _client.GetTopicsAsync( new Empty{ });
                
                return Ok(reply);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpGet("{name}", Name = "GetTopic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopics(string name)
        {
            try
            {
                var reply  = await _client.GetTopicsByNameAsync( new TopicRequest{ TopicName = name, TopicDescription = ""});
                
                return Ok(reply);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] TopicIn topicIn)
        {
            try
            {
                var reply  = await _client.AddTopicAsync( new TopicRequest{ TopicName = topicIn.Name, TopicDescription = topicIn.Description});
                
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{name}", Name = "UpdateTopic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string name, [FromBody] TopicIn topicIn)
        {
            try
            {
                var reply  = await _client.UpdateTopicAsync( new TopicRequest{ TopicName = name, TopicDescription = topicIn.Description});
                
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{name}", Name = "DeleteTopic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                var reply  = await _client.DeleteTopicAsync( new TopicRequest{ TopicName = name, TopicDescription = ""});
                
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
