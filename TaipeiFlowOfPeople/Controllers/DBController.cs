using Dapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaipeiFlowOfPeopleModel;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaipeiFlowOfPeople.Controllers
{
    /// <summary>
    /// 景點人潮
    /// </summary>
    [EnableCors("Policy1")]
    [Route("api/[controller]")]
    [ApiController]
    public class DBController : ControllerBase
    {
        private IConfiguration _config;
        public DBController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Get()
        {
            using (var cn = new TaipeiFlowOfPeopleContext(_config))
            {
            }
            return new JsonResult("");
        }

    }
}
