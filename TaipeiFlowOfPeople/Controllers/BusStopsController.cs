﻿using Dapper;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaipeiFlowOfPeople.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusStopsController : ControllerBase
    {
        string dbPath = "./SQLlite/db.sqlite";
        string cnStr = $"data source=./SQLlite/db.sqlite";

        // GET: api/<BusStopsController>
        [HttpGet]
        public IEnumerable<BusStop> Get()
        {
            this.InitSQLiteDb();
            //using (var cn = new SQLiteConnection(cnStr))
            //{
            //    var list = cn.Query<BusStop>("SELECT * FROM BusStop ;");
            //    return list;
            //}

            using (var cn = new SQLiteConnection(cnStr))
            {
                String query = "SELECT * FROM BusStop";

                using (SQLiteCommand command = new SQLiteCommand(query, cn))
                {
                    cn.Open();
                    List<BusStop> myObjectList = new List<BusStop>();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        BusStop myObject = null;
                        StopName mysn = null;
                        StopPosition mysp = null;
                        while (reader.Read())
                        {
                            myObject = new BusStop();
                            mysn = new StopName();
                            mysp = new StopPosition();

                            myObject.StopUID = reader["StopUID"].ToString();
                            myObject.StopID = reader["StopID"].ToString();
                            myObject.AuthorityID = reader["AuthorityID"].ToString();
                            myObject.StopAddress = reader["StopAddress"].ToString();
                            myObject.Bearing =reader["Bearing"].ToString();
                            myObject.StationID = reader["StationID"].ToString();
                            myObject.City = reader["City"].ToString();
                            myObject.CityCode = reader["CityCode"].ToString();
                            myObject.LocationCityCode = reader["LocationCityCode"].ToString();
                            myObject.UpdateTime = DateTime.Parse(reader["UpdateTime"].ToString());
                            myObject.VersionID = int.Parse(reader["VersionID"].ToString());

                            mysn.Zh_tw = reader["Zh_tw"].ToString();
                            mysn.En = reader["En"].ToString();
                            mysp.PositionLon = float.Parse(reader["PositionLon"].ToString());
                            mysp.PositionLat = float.Parse(reader["PositionLat"].ToString());
                            mysp.GeoHash = reader["GeoHash"].ToString();

                            myObject.StopPosition = mysp;
                            myObject.StopName = mysn;
                            myObjectList.Add(myObject);
                        }
                    }
                    //var JsonResult = JsonConvert.SerializeObject(myObjectList);
                    return myObjectList;
                }
            }
        }

        // GET api/<BusStopController>/5
        [HttpGet("{StopUID}")]
        public BusStop Get(int StationUID)
        {
            this.InitSQLiteDb();
            using (var cn = new SQLiteConnection(cnStr))
            {
                var parameters = new
                {
                    StationUID = StationUID
                };
                var entity = cn.Query<BusStop>("SELECT * FROM BusStop WHERE StopUID = @StopUID", parameters).FirstOrDefault();
                return entity;
            }
        }

        //// POST api/<BusStopsController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<BusStopsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<BusStopsController>/5
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
                cn.Execute("drop table if exists  BusStop");

                cn.Execute(@"
            CREATE TABLE BusStop (
                StopUID NVARCHAR(30),
                StopID NVARCHAR(20),
                AuthorityID NVARCHAR(100),
                StopName NVARCHAR(30),
                StopPosition NVARCHAR(50),
                StopAddress NVARCHAR(50),
                VersionID INTEGER,
                Bearing NVARCHAR(10),
                StationID NVARCHAR(10),
                City NVARCHAR(10),
                CityCode NVARCHAR(10),
                LocationCityCode NVARCHAR(10),
                UpdateTime NVARCHAR(50),
                Zh_tw NVARCHAR(20),
                En NVARCHAR(20),
                PositionLon REAL,
                PositionLat REAL,
                GeoHash REAL,
                CONSTRAINT BusStop_PK PRIMARY KEY (StopUID)
            )");

                string json = GetJsonContent("https://ptx.transportdata.tw/MOTC/v2/Bus/Stop/City/Taipei?$top=10&$format=JSON");
                //string json = GetJsonContent("https://ptx.transportdata.tw/MOTC/v2/Rail/Metro/Station/TRTC?&$format=JSON");
                Bsct bscts = Newtonsoft.Json.JsonConvert.DeserializeObject<Bsct>(json);

                ////參數是用@paramname
                var insertscript = "insert into BusStop values (@StopUID, @StopID, @AuthorityID, @StopName, @StopPosition, @StopAddress, @VersionID, @Bearing,@StationID,@City,@CityCode,@LocationCityCode,@UpdateTime,@Zh_tw,@En,@PositionLon,@PositionLat,@GeoHash)";
                //cn.Execute(insertscript, trtcs.data);

                manipulate(insertscript, bscts.data);
            }
        }


        private void manipulate(string sqlManipulate, List<BusStop> data)
        {
            using (var cn = new SQLiteConnection(cnStr))
            {
                using (SQLiteCommand command = new SQLiteCommand(sqlManipulate, cn))
                {
                    cn.Open();
                    foreach (BusStop mst in data)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@StopUID", mst.StopUID);
                        command.Parameters.AddWithValue("@StopID", mst.StopID);
                        command.Parameters.AddWithValue("@AuthorityID", mst.AuthorityID);
                        command.Parameters.AddWithValue("@StopName", mst.StopName);
                        command.Parameters.AddWithValue("@StopPosition", mst.StopPosition);
                        command.Parameters.AddWithValue("@StopAddress", mst.StopAddress);

                        command.Parameters.AddWithValue("@Bearing", mst.Bearing);
                        command.Parameters.AddWithValue("@UpdateTime", mst.UpdateTime);
                        command.Parameters.AddWithValue("@VersionID", mst.VersionID);
                        command.Parameters.AddWithValue("@StationID", mst.StationID);

                        command.Parameters.AddWithValue("@LocationCityCode", mst.LocationCityCode);
                        command.Parameters.AddWithValue("@City", mst.City);
                        command.Parameters.AddWithValue("@CityCode", mst.CityCode);
                        command.Parameters.AddWithValue("@Zh_tw", mst.StopName.Zh_tw);

                        command.Parameters.AddWithValue("@En", mst.StopName.En);
                        command.Parameters.AddWithValue("@PositionLon", mst.StopPosition.PositionLon);
                        command.Parameters.AddWithValue("@PositionLat", mst.StopPosition.PositionLat);
                        command.Parameters.AddWithValue("@GeoHash", mst.StopPosition.GeoHash);

                        int result = command.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                            Console.WriteLine("Error inserting data into BusStop Database!");
                    }
                }
            }
        }

        private string GetJsonContent(string Url)
        {
            string targetURI = Url;
            var request = System.Net.WebRequest.Create(targetURI);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            request.Headers.Add("Host", "ptx.transportdata.tw");
            //request.ContentType = "application/json; charset=utf-8";
            var response = request.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            text = "{\"success\":true,\"data\":" + text + "}";

            return text;
        }

    }
}
