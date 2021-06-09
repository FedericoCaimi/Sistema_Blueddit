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
        public async Task<IActionResult> Get([FromQuery(Name = "name")] string nameIn = null)
        {
            try
            {
                /*using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client = new Topics.TopicsClient(channel);
                var reply  = await client.GetTopicsAsync( new TopicsRequest{ Topic = nameIn});
                
                var response = reply.Message;*/

                using var channel = GrpcChannel.ForAddress("https://localhost:5003");
                var client =  new Greeter.GreeterClient(channel);
                var reply = await client.SayHelloAsync( new HelloRequest { Name = nameIn });
                var response = reply.Message;
                return Ok(response);
            }
            catch (Exception e)
            {
                //return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return Ok(e.Message);
            }
        }
    }
}
