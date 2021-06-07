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
        public TopicController()
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery(Name = "name")] string type = null)
        {
            try
            {
                var channel = GrpcChannel.ForAddress("http://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var replay  = await client.GetTopicsAsync( new TopicsRequest{ TopicName = "tema1"});
                return Ok(replay.TopicName);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
