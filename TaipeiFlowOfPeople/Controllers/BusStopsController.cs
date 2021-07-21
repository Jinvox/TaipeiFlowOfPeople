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
using System.Net;
using System.Threading.Tasks;
using TaipeiFlowOfPeopleModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaipeiFlowOfPeople.Controllers
{
    /// <summary>
    /// 公車站
    /// </summary>
    [EnableCors("Policy1")]
    [Route("api/[controller]")]
    [ApiController]
    public class BusStopsController : ControllerBase
    {
        private IConfiguration config { get; set; }
        public BusStopsController(IConfiguration configuration)
        {
            config = configuration;
        }

        /// <summary>
        /// 取得前200筆公車站牌資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<BusStop> Get()
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(BusStop));
            using(var cn = new TaipeiFlowOfPeopleContext(config))
            {
                List<BusStop> busStops = cn.BusStop.OrderBy(x => x.StopUID).Take(200).ToList();
                List<StopName> stopNames = cn.StopName.OrderBy(x=>x.Uid).Take(200).ToList();
                List<StopPosition> stopPositions = cn.StopPosition.OrderBy(x => x.Uid).Take(200).ToList();
                foreach(BusStop stop in busStops)
                {
                    StopName stopName = stopNames.FirstOrDefault(x => x.Uid == stop.StopUID);
                    stop.StopName = stopName;
                    StopPosition stopPosition = stopPositions.FirstOrDefault(x => x.Uid == stop.StopUID);
                    stop.StopPosition = stopPosition;
                }
                return busStops;
            }
        }

        /// <summary>
        /// 取得指定公車站資料
        /// </summary>
        /// <param name="stopUID">唯一識別碼</param>
        /// <returns></returns>
        [HttpGet("{stopUID}")]
        public BusStop Get(string stopUID)
        {
            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(BusStop));
            using (var cn = new TaipeiFlowOfPeopleContext(config))
            {
                BusStop entity = cn.BusStop.FirstOrDefault(x => x.StopUID == stopUID);
                StopName stopName = cn.StopName.FirstOrDefault(x => x.Uid == stopUID);
                StopPosition stopPosition = cn.StopPosition.FirstOrDefault(x => x.Uid == stopUID);
                entity.StopName = stopName;
                entity.StopPosition = stopPosition;
                return entity;
            }
        }

        /// <summary>
        /// 依據指定的座標和範圍取得公車站資料，最多 200 筆
        /// </summary>
        /// <param name="positionLon">經度位置</param>
        /// <param name="positionLat">緯度位置</param>
        /// <param name="rangeLon">經度範圍</param>
        /// <param name="rangeLat">緯度範圍</param>
        /// <returns></returns>
        [HttpGet("byPosition")]
        public IEnumerable<BusStop> Get(float? positionLon, float? positionLat, float? rangeLon, float? rangeLat)
        {
            if (positionLon == null || positionLat == null)
                return new List<BusStop>();

            TaipeiFlowOfPeopleContext.CheckAndInitial(config, nameof(BusStop));
            using (var cn = new TaipeiFlowOfPeopleContext(config))
            {
                var tempStopPositions = cn.StopPosition.Where(x =>
                x.PositionLon >= (positionLon.Value - rangeLon ?? 0.001f) &&
                x.PositionLon <= (positionLon.Value + rangeLon ?? 0.001f) &&
                x.PositionLat >= (positionLat.Value - rangeLat ?? 0.001f) &&
                x.PositionLat <= (positionLat.Value + rangeLat ?? 0.001f)
                ).OrderBy(x => x.Uid).Take(200);
                List<StopPosition> stopPositions = tempStopPositions.ToList();
                List<BusStop> busStops = cn.BusStop.Join(tempStopPositions, x => x.StopUID, y => y.Uid, (x, y) => x).ToList();
                List<StopName> stopNames = cn.StopName.Join(tempStopPositions, x => x.Uid, y => y.Uid, (x, y) => x).ToList();
                
                foreach(BusStop busStop in busStops)
                { 
                    StopName stopName = stopNames.FirstOrDefault(x => x.Uid == busStop.StopUID);
                    busStop.StopName = stopName;
                    StopPosition stopPosition = stopPositions.FirstOrDefault(x => x.Uid == busStop.StopUID);
                    busStop.StopPosition = stopPosition;
                }
                return busStops;
            }
        }
    }
}
