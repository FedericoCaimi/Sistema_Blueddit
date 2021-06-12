using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Grpc.Net.Client;

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
        public async Task<IActionResult> GetTopics([FromQuery(Name = "name")] string nameIn = null)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.GetTopicsAsync( new GetTopicByNameRequest{ TopicName = nameIn});
                
                var response = reply.Json;

                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
    }
}
