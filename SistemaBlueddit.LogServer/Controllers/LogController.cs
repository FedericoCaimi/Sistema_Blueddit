using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SistemaBlueddit.LogServer.Logic.Interfaces;
using Microsoft.AspNetCore.Http;

namespace SistemaBlueddit.LogServer.Controllers
{
    [ApiController]
    [Route("log")]
    public class LogController : ControllerBase
    {
        private ILogLogic _logLogic;

        public LogController(ILogLogic logLogic) : base()
        {
            _logLogic = logLogic;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get([FromQuery(Name = "type")] string type = null, [FromQuery(Name = "date")] string dateString = null)
        {
            try
            {
                var logs = _logLogic.GetAll(type, dateString).ToList();
                return Ok(logs);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
