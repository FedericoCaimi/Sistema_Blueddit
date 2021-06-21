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

        private GrpcChannel _channel;

        public PostController(IConfiguration configuration)
        {
            _configuration = configuration;
            _serverAddress = _configuration.GetSection("serverAddress").Value;
            _channel = GrpcChannel.ForAddress(_serverAddress);
            _client = new Posts.PostsClient(_channel);
        }

        /// <summary>
        /// Permite a un usuario obtener todas las publicaciones del sistema
        /// </summary>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                _channel.Dispose();
                return Ok(response);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Permite a un usuario obtener una publicación con el nombre que se pase por parámetro
        /// </summary>
        /// <param name="name">Este parámetro contiene el nombre del post a retornar</param>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

                if(response.Posts.Count == 0)
                {
                    return BadRequest(response.Message);
                }
                _channel.Dispose();
                return Ok(response);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Permite a un usuario agregar una nueva publicación al sistema
        /// </summary>
        /// <param name="postIn">Este parámetro contiene la información de la publicación a agregar</param>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPost([FromBody] PostIn postIn)
        {
            try
            {
                var reply  = await _client.AddPostAsync( new PostRequest{ Name = postIn.Name, Content = postIn.Content, Topics = {GrpcMapperHelper.ToGrpcTopics(postIn.Topics)}});

                if(reply.Posts.Count == 0)
                {
                    return BadRequest(reply.Message);
                }
                
                var response = new PostOut
                {
                    Posts = GrpcMapperHelper.ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                _channel.Dispose();
                return Ok(response);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Permite a un usuario actualizar una publicación del sistema
        /// </summary>
        /// <param name="postIn">Este parámetro contiene la información de la publicación a actualizar</param>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePost([FromBody] PostIn postIn)
        {
            try
            {
                var reply  = await _client.UpdatePostAsync( new PostRequest{ Name = postIn.Name, Content = postIn.Content, Topics = { GrpcMapperHelper.ToGrpcTopics(postIn.Topics)}});

                if(reply.Posts.Count == 0)
                {
                    return BadRequest(reply.Message);
                }
                
                var response = new PostOut
                {
                    Posts = GrpcMapperHelper.ToDomainPost(reply.Posts),
                    Message = reply.Message
                };
                _channel.Dispose();
                return Ok(response);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Permite a un usuario borrar una publicación del sistema
        /// </summary>
        /// <param name="name">Este parámetro contiene el nombre de la publicación a borrar</param>
        /// <response code="200">Se devuelve la información requerida.</response>
        /// <response code="400">Error de solicitud del cliente</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpDelete("{name}", Name = "DeletePost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                var reply  = await _client.DeletePostAsync( new PostRequest{ Name = name});

                if (reply.Posts.Count == 0)
                {
                    return BadRequest(reply.Message);
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
