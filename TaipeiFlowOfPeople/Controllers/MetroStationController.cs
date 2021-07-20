using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TaipeiFlowOfPeople.Model;

namespace TaipeiFlowOfPeople.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetroStationController : ControllerBase
    {
        string dbPath = "./SQLlite/db.sqlite";
        string cnStr = $"data source=./SQLlite/db.sqlite";

        // GET: api/<AttractionsController>
        [HttpGet]
        public IEnumerable<Attraction> Get()
        {
            this.InitSQLiteDb();
            using (var cn = new SQLiteConnection(cnStr))
            {
                var list = cn.Query<Attraction>("SELECT * FROM MetroStation");
                return list;
            }
        }

        // GET api/<AttractionsController>/5
        [HttpGet("{id}")]
        public Attraction Get(int id)
        {
            this.InitSQLiteDb();
            using (var cn = new SQLiteConnection(cnStr))
            {
                var parameters = new
                {
                    id = id
                };
                var entity = cn.Query<Attraction>("SELECT * FROM Attraction WHERE id = @id", parameters).FirstOrDefault();
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

        private void InitSQLiteDb()
        {
            //if (System.IO.File.Exists(dbPath))
            //    return;
            using (var cn = new SQLiteConnection(cnStr))
            {

                /* public string StationUID
         public string StationID
         public StationName StationName
         public string StationAddress
         public bool BikeAllowOnHoliday
         public DateTime SrcUpdateTime
         public DateTime UpdateTime
         public int VersionID
         public StationPosition StationPosition
         public string LocationCity
         public string LocationCityCode
         public string LocationTown
         public string LocationTownCode*/

                cn.Execute("drop table if exists  MetroStation");
                cn.Execute("drop table if exists StationName");
                cn.Execute("drop table if exists StationPosition");

                cn.Execute(@"
            CREATE TABLE MetroStation (
                StationUID NVARCHAR(30),
                StationID NVARCHAR(20),
                StationAddress NVARCHAR(100),
                BikeAllowOnHoliday REAL,
                SrcUpdateTime NVARCHAR(50),
                UpdateTime NVARCHAR(50),
                VersionID INTEGER,
                LocationCity NVARCHAR(10),
                LocationCityCode NVARCHAR(10),
                LocationTown NVARCHAR(10),
                LocationTownCode NVARCHAR(10),
                CONSTRAINT MetroStation_PK PRIMARY KEY (StationUID)
            )");

                            cn.Execute(@"
            CREATE TABLE StationName (
                StationUID NVARCHAR(30),
                Zh_tw NVARCHAR(20),
                En NVARCHAR(20),
                CONSTRAINT MetroStation_PK PRIMARY KEY (StationUID)
            )");

                            cn.Execute(@"
            CREATE TABLE StationPosition (
                StationUID NVARCHAR(30),
                PositionLon REAL,
                GeoHash REAL,
                CONSTRAINT MetroStation_PK PRIMARY KEY (StationUID)
            )");



                /*    cn.Execute(@"
    CREATE TABLE Attraction (
        id INTEGER,
        name NVARCHAR(50),
        district NVARCHAR(50),
        level INTEGER,
        nlat REAL,
        elong REAL,
        cover NVARCHAR(100),
        update_time NVARCHAR(50),
        CONSTRAINT Attraction_PK PRIMARY KEY (id)
    )");*/
                //string json = GetJsonContent("https://ptx.transportdata.tw/MOTC/v2/Rail/Metro/Station/TRTC?$top=30&$format=JSON");
                string json = GetJsonContent("https://ptx.transportdata.tw/MOTC/v2/Rail/Metro/Station/TRTC");
                MetroStation metrostations = Newtonsoft.Json.JsonConvert.DeserializeObject<MetroStation>(json);
                //cn.execute("delete from metrostation");
                //cn.execute("delete from stationname");
                //cn.execute("delete from stationposition");
                ////參數是用@paramname
                //var insertscript =
                //    "insert into attraction values (@id, @name, @district, @level, @nlat, @elong, @cover, @update_time)";
                //cn.execute(insertscript, spots.data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        private string GetJsonContent(string Url)
        {
            string targetURI = Url;
            //HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(targetURI);
            //request.ContentType = "application/json; charset=utf-8";
            //webrequest.Accept = "application/json";
            //webrequest.ContentType = "application/json";
            //HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();

            var request = System.Net.WebRequest.Create(targetURI);
            //request.ContentType = "application/json; charset=utf-8";
            var response = request.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }
            return text;
        }


    }
}
