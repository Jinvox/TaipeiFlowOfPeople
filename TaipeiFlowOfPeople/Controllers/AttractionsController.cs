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
        private IConfiguration config { get; set; }
        public AttractionsController(IConfiguration configuration)
        {
            config = configuration;
        }

        /// <summary>
        /// 取得所有景點人潮資料最多200筆
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Attraction> Get()
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entities = context.Attraction.ToList();
                return entities;
            }
        }

        /// <summary>
        /// 取得指定編號的景點人朝資料
        /// </summary>
        /// <param name="id">唯一識別編號</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Attraction Get(int id)
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entity = context.Attraction.Where(x => x.id == id).FirstOrDefault();
                return entity;
            }
        }

        /// <summary>
        /// 以名稱或行政區模糊搜索景點
        /// </summary>
        /// <param name="nameOrDistrict">名稱或行政區</param>
        /// <returns></returns>
        [HttpGet("byNameDistrict")]
        public IEnumerable<Attraction> Get(string nameOrDistrict)
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entities = context.Attraction.Where(x=>x.name.Contains(nameOrDistrict) || x.district.Contains(nameOrDistrict)).Take(200).ToList();
                return entities;
            }
        }

        /// <summary>
        /// 以名稱過濾景點
        /// </summary>
        /// <param name="name">名稱</param>
        /// <returns></returns>
        [HttpGet("byName")]
        public IEnumerable<Attraction> GetByName(string name)
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entities = context.Attraction.Where(x => x.name == name).Take(200).ToList();
                return entities;
            }
        }

        /// <summary>
        /// 以行政區過濾景點
        /// </summary>
        /// <param name="district">行政區</param>
        /// <returns></returns>
        [HttpGet("byDistrict")]
        public IEnumerable<Attraction> GetByDistrict(string district)
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entities = context.Attraction.Where(x => x.district == district).Take(200).ToList();
                return entities;
            }
        }

        /// <summary>
        /// 以人潮等級過濾景點
        /// </summary>
        /// <param name="level">人潮等級</param>
        /// <returns></returns>
        [HttpGet("byLevel")]
        public IEnumerable<Attraction> GetByLevel(int level)
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entities = context.Attraction.Where(x => x.level == level).Take(200).ToList();
                return entities;
            }
        }

        /// <summary>
        /// 以燈號+人潮等級過濾景點
        /// </summary>
        /// <param name="districtslevels">行政區+人潮等級</param>
        /// <returns></returns>
        /// 
        [HttpPost("byDistrictsLevels")]
        public IEnumerable<Attraction> GetByDistrictsLevels(GetRequestParameters districtslevels)
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(Attraction));
            //var districts = new List<string> { "文山區", "北投區" };
            //var levels = new List<int> { 1 };
            var districts = districtslevels.Districts.ToList();
            var levels = districtslevels.Levels.ToList();
            using (var context = new TaipeiFlowOfPeopleContext(config))
            {
                var entities = context.Attraction.Where(x => levels.Contains((int)x.level) && districts.Contains((string)x.district)).Take(200).ToList();
                return entities;
            }
        }


        [BindProperties]
        public class GetRequestParameters
        {
            [BindProperty]
            public string[] Districts { get; set; }
            [BindProperty]
            public int[] Levels { get; set; }
        }

    }
}
