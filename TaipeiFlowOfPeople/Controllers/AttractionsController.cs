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
//celine test
namespace TaipeiFlowOfPeople.Controllers
{
    /// <summary>
    /// 景點人潮
    /// </summary>
    [EnableCors("Policy1")]
    [Route("api/[controller]")]
    [ApiController]
    public class AttractionsController : ControllerBase
    {
        private string dbPath { get; set; }
        private string cnStr { get; set; }
        private IConfiguration config;
        public AttractionsController(IConfiguration configuration)
        {
            dbPath = $"{AppDomain.CurrentDomain.BaseDirectory}SQLlite/db.sqlite";
            cnStr = $"data source={dbPath}";
            config = configuration;
        }
    

    // GET: api/<AttractionsController>
    /// <summary>
    /// 取得所有景點人潮資料
    /// </summary>
    /// <returns></returns>
    [HttpGet]
        public IEnumerable<Attraction> Get()
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            Console.WriteLine($"Connect to {cnStr}");
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entities = context.Attraction.ToList();
                return entities;
            }
        }

        // GET api/<AttractionsController>/5
        /// <summary>
        /// 取得指定 id 的景點人朝資料
        /// </summary>
        /// <param name="id">指定 id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Attraction Get(int id)
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            Console.WriteLine($"Connect to {cnStr}");
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entity = context.Attraction.Where(x=>x.id == id).FirstOrDefault();
                return entity;
            }
        }

        //// POST api/<AttractionsController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<AttractionsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<AttractionsController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
