using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaipeiFlowOfPeopleModel;

namespace TaipeiFlowOfPeople.Controllers
{
    [EnableCors("Policy1")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class YouBikeTPController : ControllerBase
    {
        private IConfiguration config { get; set; }
        private TaipeiFlowOfPeopleContext context { get; set; }
        public YouBikeTPController(IConfiguration configuration, TaipeiFlowOfPeopleContext taipeiFlowOfPeopleContext)
        {
            config = configuration;
            context = taipeiFlowOfPeopleContext;
        }

        /// <summary>
        /// 取得所有 YouBike 站點，最多200筆
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<YouBikeStation> Get()
        {
            List<YouBikeStation> busStops = context.YouBikeStation.OrderBy(x => x.sno).Take(200).ToList();
            return busStops;
        }

        /// <summary>
        /// 以編號 sno 取得指定 YouBike 站點
        /// </summary>
        /// <param name="sno">唯一識別碼</param>
        /// <returns></returns>
        [HttpGet("{sno}")]
        public YouBikeStation Get(string sno)
        {
            YouBikeStation entity = context.YouBikeStation.FirstOrDefault(x => x.sno == sno);
            return entity;
        }

        /// <summary>
        /// 以經緯度查找 YouBike 站點
        /// </summary>
        /// <param name="positionLng">經度位置</param>
        /// <param name="positionLat">緯度位置</param>
        /// <param name="rangeLng">經度範圍</param>
        /// <param name="rangeLat">緯度範圍</param>
        /// <returns></returns>
        [HttpGet("byPosition")]
        public IEnumerable<YouBikeStation> GetByPosition(float? positionLng, float? positionLat, float? rangeLng, float? rangeLat)
        {
            List<YouBikeStation> youBikeStations = context.YouBikeStation.Where(x =>
                        x.lngPosition >= (positionLng.Value - rangeLng ?? 0.001f) &&
                        x.lngPosition <= (positionLng.Value + rangeLng ?? 0.001f) &&
                        x.latPosition >= (positionLat.Value - rangeLat ?? 0.001f) &&
                        x.latPosition <= (positionLat.Value + rangeLat ?? 0.001f)
                        ).OrderBy(x => x.sno).Take(200).ToList();
            return youBikeStations;
        }
    }
}
