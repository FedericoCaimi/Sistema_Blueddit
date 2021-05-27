using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaBlueddit.Domain;
using SistemaBlueddit.LogServer.Logic.Interfaces;
using Microsoft.AspNetCore.Http;

namespace SistemaBlueddit.LogServer.Controllers
{
    [ApiController]
    [Route("[post]")]
    public class PostController : ControllerBase
    {

        private IPostLogLogic PostLogic;
        public PostController(IPostLogLogic postLogic) : base()
        {
            this.PostLogic = postLogic;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            try
            {
                List<Log> postLogs = this.PostLogic.GetAll().ToList(); ;
                return Ok(postLogs);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
