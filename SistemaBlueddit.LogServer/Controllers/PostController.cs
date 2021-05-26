using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SistemaBlueddit.Domain;

namespace SistemaBlueddit.LogServer.Controllers
{
    [ApiController]
    [Route("[post]")]
    public class PostController : ControllerBase
    {

        private IPostLogic PostLogic;
        public PostController(IPostLogic postLogic) : base()
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
