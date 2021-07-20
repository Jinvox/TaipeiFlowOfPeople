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

        // GET: api/<MetroStationsController>
        [HttpGet]
        public IEnumerable<MetroStation> Get()
        {
            this.InitSQLiteDb();
            //using (var cn = new SQLiteConnection(cnStr))
            //{

            //    var list = cn.Query<MetroStation>("SELECT * FROM MetroStation ;");
            //    return list;
            //}

            using (var cn = new SQLiteConnection(cnStr))
            {
                String query = "SELECT * FROM MetroStation";

                using (SQLiteCommand command = new SQLiteCommand(query, cn))
                {
                    cn.Open();
                    List<MetroStation> myObjectList = new List<MetroStation>();
                    var reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        MetroStation myObject = null;
                        StationName mysn = null;
                        StationPosition mysp=null;
                        while (reader.Read())
                        {
                            myObject = new MetroStation();
                            mysn = new StationName();
                            mysp = new StationPosition();

                            //[StationUID],[StationID],[StationAddress],
                            //[BikeAllowOnHoliday],[SrcUpdateTime],[UpdateTime]
                            //,[VersionID],[LocationCity],[LocationCityCode]
                            //,[LocationTown],[LocationTownCode]
                            //,[Zh_tw] as 'StationName.Zh_tw'
                            //,[En] as 'StationName.En'
                            //,[PositionLon] as 'StationPosition.PositionLon'
                            //,[PositionLat] as 'StationPosition.PositionLat'
                            //,[GeoHash] as 'StationPosition.GeoHash'

                            myObject.StationUID = reader["StationUID"].ToString();
                            myObject.StationID = reader["StationID"].ToString();
                            myObject.StationAddress = reader["StationAddress"].ToString();
                            myObject.BikeAllowOnHoliday = reader["BikeAllowOnHoliday"].ToString()=="1"?true:false;
                            myObject.SrcUpdateTime = DateTime.Parse(reader["SrcUpdateTime"].ToString());
                            myObject.UpdateTime = DateTime.Parse(reader["UpdateTime"].ToString());
                            myObject.VersionID = int.Parse(reader["VersionID"].ToString());
                            myObject.LocationCity = reader["LocationCity"].ToString();
                            myObject.LocationCityCode = reader["LocationCityCode"].ToString();
                            myObject.LocationTown = reader["LocationTown"].ToString();
                            myObject.LocationTownCode = reader["LocationTownCode"].ToString();

                            mysn.Zh_tw = reader["Zh_tw"].ToString();
                            mysn.En = reader["En"].ToString();
                            mysp.PositionLon = float.Parse(reader["PositionLon"].ToString());
                            mysp.PositionLat = float.Parse(reader["PositionLat"].ToString());
                            mysp.GeoHash = reader["GeoHash"].ToString();
                            myObject.StationPosition = mysp;
                            myObject.StationName = mysn;
                            myObjectList.Add(myObject);
                        }
                    }
                    //var JsonResult = JsonConvert.SerializeObject(myObjectList);
                    return myObjectList;
                }
            }



        }

        // GET api/<MetroStationController>/5
        [HttpGet("{StationUID}")]
        public MetroStation Get(int StationUID)
        {
            this.InitSQLiteDb();
            using (var cn = new SQLiteConnection(cnStr))
            {
                var parameters = new
                {
                    StationUID = StationUID
                };
                var entity = cn.Query<MetroStation>("SELECT * FROM MetroStation WHERE StationUID = @StationUID", parameters).FirstOrDefault();
                return entity;
            }
        }

        //// POST api/<MetroStationsController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<MetroStationsController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<MetroStationsController>/5
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
                //cn.Execute("drop table if exists StationName");
                //cn.Execute("drop table if exists StationPosition");

                //    cn.Execute(@"
                //CREATE TABLE MetroStation (
                //    StationUID NVARCHAR(30),
                //    StationID NVARCHAR(20),
                //    StationAddress NVARCHAR(100),
                //    BikeAllowOnHoliday REAL,
                //    SrcUpdateTime NVARCHAR(50),
                //    UpdateTime NVARCHAR(50),
                //    VersionID INTEGER,
                //    LocationCity NVARCHAR(10),
                //    LocationCityCode NVARCHAR(10),
                //    LocationTown NVARCHAR(10),
                //    LocationTownCode NVARCHAR(10),
                //    CONSTRAINT MetroStation_PK PRIMARY KEY (StationUID)
                //)");

                //                cn.Execute(@"
                //CREATE TABLE StationName (
                //    StationUID NVARCHAR(30),
                //    Zh_tw NVARCHAR(20),
                //    En NVARCHAR(20),
                //    CONSTRAINT MetroStation_PK PRIMARY KEY (StationUID)
                //)");

                //                cn.Execute(@"
                //CREATE TABLE StationPosition (
                //    StationUID NVARCHAR(30),
                //    PositionLon REAL,
                //    GeoHash REAL,
                //    CONSTRAINT MetroStation_PK PRIMARY KEY (StationUID)
                //)");


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
                Zh_tw NVARCHAR(20),
                En NVARCHAR(20),
                PositionLon REAL,
                PositionLat REAL,
                GeoHash REAL,
                CONSTRAINT MetroStation_PK PRIMARY KEY (StationUID)
            )");

                /*    cn.Execute(@"
    CREATE TABLE MetroStation(
        id INTEGER,
        name NVARCHAR(50),
        district NVARCHAR(50),
        level INTEGER,
        nlat REAL,
        elong REAL,
        cover NVARCHAR(100),
        update_time NVARCHAR(50),
        CONSTRAINT MetroStation_PK PRIMARY KEY (id)
    )");*/
                string json = GetJsonContent("https://ptx.transportdata.tw/MOTC/v2/Rail/Metro/Station/TRTC?$top=2&$format=JSON");
                //string json = GetJsonContent("https://ptx.transportdata.tw/MOTC/v2/Rail/Metro/Station/TRTC");
                Trtc trtcs = Newtonsoft.Json.JsonConvert.DeserializeObject<Trtc>(json);

                ////參數是用@paramname
                var insertscript = "insert into MetroStation values (@StationUID, @StationID, @StationAddress, @BikeAllowOnHoliday, @SrcUpdateTime, @UpdateTime, @VersionID, @LocationCity,@LocationCityCode,@LocationTown,@LocationTownCode,@Zh_tw,@En,@PositionLon,@PositionLat,@GeoHash)";
                //cn.Execute(insertscript, trtcs.data);

                manipulate(insertscript, trtcs.data);

            //    cn.Execute(@"
            //CREATE TABLE MetroStation (
            //    StationUID NVARCHAR(30),
            //    StationID NVARCHAR(20),
            //    StationAddress NVARCHAR(100),
            //    BikeAllowOnHoliday REAL,
            //    SrcUpdateTime NVARCHAR(50),
            //    UpdateTime NVARCHAR(50),
            //    VersionID INTEGER,
            //    LocationCity NVARCHAR(10),
            //    LocationCityCode NVARCHAR(10),
            //    LocationTown NVARCHAR(10),
            //    LocationTownCode NVARCHAR(10),
            //    Zh_tw NVARCHAR(20),
            //    En NVARCHAR(20),
            //    PositionLon REAL,
            //    GeoHash REAL,
            //    CONSTRAINT MetroStation_PK PRIMARY KEY (StationUID)
            //)");
            }
        }


        private void manipulate(string sqlManipulate, List<MetroStation> data)
        {
            using (var cn = new SQLiteConnection(cnStr))
            {
                    using (SQLiteCommand command = new SQLiteCommand(sqlManipulate,cn))
                    {
                        cn.Open();
                        foreach (MetroStation mst in data)
                        {
                            command.Parameters.Clear();
                            //var insertscript = "insert into MetroStation values
                            //(@StationUID, @StationID, @StationAddress, @BikeAllowOnHoliday,
                            //@SrcUpdateTime, @UpdateTime, @VersionID, @LocationCity,
                            //@LocationCityCode,@LocationTown,@LocationTownCode,@Zh_tw,@En,
                            //@PositionLon,@GeoHash)";
                            command.Parameters.AddWithValue("@StationUID", mst.StationUID);
                            command.Parameters.AddWithValue("@StationID", mst.StationID);
                            command.Parameters.AddWithValue("@StationAddress", mst.StationAddress);
                            command.Parameters.AddWithValue("@BikeAllowOnHoliday", mst.BikeAllowOnHoliday);

                            command.Parameters.AddWithValue("@SrcUpdateTime", mst.SrcUpdateTime);
                            command.Parameters.AddWithValue("@UpdateTime", mst.UpdateTime);
                            command.Parameters.AddWithValue("@VersionID", mst.VersionID);
                            command.Parameters.AddWithValue("@LocationCity", mst.LocationCity);

                            command.Parameters.AddWithValue("@LocationCityCode", mst.LocationCityCode);
                            command.Parameters.AddWithValue("@LocationTown", mst.LocationTown);
                            command.Parameters.AddWithValue("@LocationTownCode", mst.LocationTownCode);
                            command.Parameters.AddWithValue("@Zh_tw", mst.StationName.Zh_tw);

                            command.Parameters.AddWithValue("@En", mst.StationName.En);
                            command.Parameters.AddWithValue("@PositionLon", mst.StationPosition.PositionLon);
                            command.Parameters.AddWithValue("@PositionLat", mst.StationPosition.PositionLat);
                            command.Parameters.AddWithValue("@GeoHash", mst.StationPosition.GeoHash);

                            
                            int result = command.ExecuteNonQuery();

                            // Check Error
                            if (result < 0)
                                Console.WriteLine("Error inserting data into Database!");
                        }
                }
                
            }

                

            //using (var cn = new SQLiteConnection(cnStr))
            //{
            //    var command = new SQLiteCommand(sqlManipulate, cn);
            //    var mySqlTransaction = cn.BeginTransaction();

            //    try
            //    {
            //        command.Transaction = mySqlTransaction;
            //        command.ExecuteNonQuery();
            //        mySqlTransaction.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        mySqlTransaction.Rollback();
            //        Console.WriteLine(ex.Message);
            //    }
            //}
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
