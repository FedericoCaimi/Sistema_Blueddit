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

        private GrpcChannel _channel;

        public TopicController(IConfiguration configuration) 
        {
            _configuration = configuration;
            _serverAddress = _configuration.GetSection("serverAddress").Value;
            _channel = GrpcChannel.ForAddress(_serverAddress);
            _client = new Topics.TopicsClient(_channel);
        }

        /// <summary>
        /// Permite a un usuario obtener todos los temas del sistema
        /// </summary>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopics()
        {
            try
            {
                var reply  = await _client.GetTopicsAsync( new Empty{ });
                _channel.Dispose();
                return Ok(reply);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Permite a un usuario obtener un tema del sistema mediante su nombre ingresado por parámetro
        /// </summary>
        /// <param name="name">Este parámetro contiene el nombre del tema a obtener</param>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{name}", Name = "GetTopic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopics(string name)
        {
            try
            {
                var reply  = await _client.GetTopicsByNameAsync( new TopicRequest{ TopicName = name, TopicDescription = ""});

                if (reply.Topics.Count == 0)
                {
                    return BadRequest(reply);
                }
                _channel.Dispose();
                return Ok(reply);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Permite a un usuario crear un nuevo tema y agregarlo al sistema
        /// </summary>
        /// <param name="topicIn">Este parámetro contiene la información del nuevo tema</param>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] TopicIn topicIn)
        {
            try
            {
                var reply  = await _client.AddTopicAsync( new TopicRequest{ TopicName = topicIn.Name, TopicDescription = topicIn.Description});

                if (reply.Topics.Count == 0)
                {
                    return BadRequest(reply);
                }
                _channel.Dispose();
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Permite a un usuario actualizar un tema del sistema
        /// </summary>
        /// <param name="name">Este parámetro contiene el nombre del tema a actualizar</param>
        /// <param name="topicIn">Este parámetro contiene la información del tema a actualizar</param>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut("{name}", Name = "UpdateTopic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string name, [FromBody] TopicIn topicIn)
        {
            try
            {
                var reply  = await _client.UpdateTopicAsync( new TopicRequest{ TopicName = name, TopicDescription = topicIn.Description});

                if (reply.Topics.Count == 0)
                {
                    return BadRequest(reply);
                }
                _channel.Dispose();
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Permite a un usuario borrar un tema del sistema
        /// </summary>
        /// <param name="name">Este parámetro contiene el nombre del tema a borrar</param>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{name}", Name = "DeleteTopic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                var reply  = await _client.DeleteTopicAsync( new TopicRequest{ TopicName = name, TopicDescription = ""});

                if (reply.Topics == null || reply.Topics.Count == 0)
                {
                    return BadRequest(reply);
                }
                _channel.Dispose();
                return Ok(reply);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
