using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaipeiFlowOfPeople.Model;

namespace TaipeiFlowOfPeople.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YouBikeTPController : ControllerBase
    {
        string dbPath = "./SQLlite/db.sqlite";
        string cnStr = $"data source=./SQLlite/db.sqlite";

        // GET: api/<YouBikeTPController>
        [HttpGet]
        public IEnumerable<YouBikeStation> Get()
        {
            this.InitSQLiteDb();
            using (var cn = new SQLiteConnection(cnStr))
            {
                var list = cn.Query<YouBikeStation>("SELECT * FROM YouBikeStation");
                return list;
            }
        }

        // GET api/<YouBikeTPController>/5
        [HttpGet("{sno}")]
        public YouBikeStation Get(string sno)
        {
            //this.InitSQLiteDb();
            using (var cn = new SQLiteConnection(cnStr))
            {
                var parameters = new
                {
                    sno = sno
                };
                var entity = cn.Query<YouBikeStation>("SELECT * FROM YouBikeStation WHERE sno = @sno", parameters).FirstOrDefault();
                return entity;
            }
        }

        private void InitSQLiteDb()
        {
            //if (System.IO.File.Exists(dbPath))
            //    return;
            using (var cn = new SQLiteConnection(cnStr))
            {
                string json = GetJsonContent("https://tcgbusfs.blob.core.windows.net/blobyoubike/YouBikeTP.json");

                YouBikeTP youbiketp = Newtonsoft.Json.JsonConvert.DeserializeObject<YouBikeTP>(json);

                cn.Execute("drop table if exists  YouBikeStation");

                cn.Execute(@"
                CREATE TABLE YouBikeStation (
                    sno NVARCHAR(5),
                    sna NVARCHAR(50),
                    tot NVARCHAR(5),
                    sbi NVARCHAR(5),
                    sarea NVARCHAR(10),
                    mday NVARCHAR(14),
                    lat NVARCHAR(15),
                    lng NVARCHAR(15),
                    ar NVARCHAR(100),
                    sareaen NVARCHAR(50),
                    snaen NVARCHAR(100),
                    aren NVARCHAR(100),
                    bemp NVARCHAR(3),
                    act NVARCHAR(1),
                    CONSTRAINT YouBikeStation_PK PRIMARY KEY (sno)
                )");

                //cn.Execute("DELETE FROM YouBikeStation");
                ////參數是用@paramName
                var insertScript =
                    "INSERT INTO YouBikeStation VALUES (@sno, @sna, @tot, @sbi, @sarea, @mday, @lat, @lng, @ar, @sareaen, @snaen, @aren, @bemp, @act)";
                cn.Execute(insertScript, youbiketp.retVal);
            }
        }

        private string GetJsonContent(string Url)
        {
            string targetURI = Url;
            var request = System.Net.WebRequest.Create(targetURI);
            request.ContentType = "application/json; charset=utf-8";
            var response = request.GetResponse();
            string text;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            JObject json = JObject.Parse(text);
            JObject coins = (JObject)json["retVal"];
            JArray array = new JArray();
            foreach (JProperty property in coins.Properties())
            {
                string name = property.Name;
                array.Add(property.Value);
            }
            json["retVal"]= array;
            text=json.ToString();

            return text;
        }

    }
}
