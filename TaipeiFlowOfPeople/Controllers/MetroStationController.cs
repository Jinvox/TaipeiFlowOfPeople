using Dapper;
using Microsoft.AspNetCore.Authorization;
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

namespace TaipeiFlowOfPeople.Controllers
{
    /// <summary>
    /// 捷运站
    /// </summary>
    [EnableCors("Policy1")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MetroStationController : ControllerBase
    {
        private IConfiguration config { get; set; }
        private TaipeiFlowOfPeopleContext context { get; set; }
        public MetroStationController(IConfiguration configuration, TaipeiFlowOfPeopleContext taipeiFlowOfPeopleContext)
        {
            config = configuration;
            context = taipeiFlowOfPeopleContext;
        }

        /// <summary>
        /// 取得前两百比捷运站资料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<MetroStation> Get()
        {
            List<MetroStation> metroStations = context.MetroStation.OrderBy(x => x.StationUID).Take(200).ToList();
            List<StationName> stopNames = context.StationName.OrderBy(x => x.Uid).Take(200).ToList();
            List<StationPosition> stopPositions = context.StationPosition.OrderBy(x => x.Uid).Take(200).ToList();
            foreach (MetroStation stop in metroStations)
            {
                StationName stopName = stopNames.FirstOrDefault(x => x.Uid == stop.StationUID);
                stop.StationName = stopName;
                StationPosition stopPosition = stopPositions.FirstOrDefault(x => x.Uid == stop.StationUID);
                stop.StationPosition = stopPosition;
            }
            return metroStations;
        }

        /// <summary>
        /// 取得指定捷運站資料
        /// </summary>
        /// <param name="stationUID">唯一識別碼</param>
        /// <returns></returns>
        [HttpGet("{stationUID}")]
        public MetroStation Get(string stationUID)
        {
            MetroStation entity = context.MetroStation.FirstOrDefault(x => x.StationUID == stationUID);
            StationName name = context.StationName.FirstOrDefault(x => x.Uid == stationUID);
            StationPosition position = context.StationPosition.FirstOrDefault(x => x.Uid == stationUID);
            entity.StationName = name;
            entity.StationPosition = position;
            return entity;
        }

        /// <summary>
        /// 依據指定的座標和範圍取得捷運站資料，最多 200 筆
        /// </summary>
        /// <param name="positionLon">經度位置</param>
        /// <param name="positionLat">緯度位置</param>
        /// <param name="rangeLon">經度範圍</param>
        /// <param name="rangeLat">緯度範圍</param>
        /// <returns></returns>
        [HttpGet("byPosition")]
        public IEnumerable<MetroStation> Get(float? positionLon, float? positionLat, float? rangeLon, float? rangeLat)
        {
            if (positionLon == null || positionLat == null)
                return new List<MetroStation>();

            var tempStationPositions = context.StationPosition.Where(x =>
            x.PositionLon >= (positionLon.Value - rangeLon ?? 0.001f) &&
            x.PositionLon <= (positionLon.Value + rangeLon ?? 0.001f) &&
            x.PositionLat >= (positionLat.Value - rangeLat ?? 0.001f) &&
            x.PositionLat <= (positionLat.Value + rangeLat ?? 0.001f)
            ).OrderBy(x => x.Uid).Take(200);
            List<StationPosition> stationPositions = tempStationPositions.ToList();
            List<MetroStation> MetroStations = context.MetroStation.Join(tempStationPositions, x => x.StationUID, y => y.Uid, (x, y) => x).ToList();
            List<StationName> stationNames = context.StationName.Join(tempStationPositions, x => x.Uid, y => y.Uid, (x, y) => x).ToList();

            foreach (MetroStation busStop in MetroStations)
            {
                StationName stopName = stationNames.FirstOrDefault(x => x.Uid == busStop.StationUID);
                busStop.StationName = stopName;
                StationPosition stopPosition = stationPositions.FirstOrDefault(x => x.Uid == busStop.StationUID);
                busStop.StationPosition = stopPosition;
            }
            return MetroStations;
        }
    }
}
