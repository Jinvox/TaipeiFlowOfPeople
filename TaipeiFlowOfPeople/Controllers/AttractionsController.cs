using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaipeiFlowOfPeople.Model;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//testtest
//testtest
//testtest3
namespace TaipeiFlowOfPeople.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttractionsController : ControllerBase
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
                var list = cn.Query<Attraction>("SELECT * FROM Attraction");
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
            if (System.IO.File.Exists(dbPath)) 
                return;
            using (var cn = new SQLiteConnection(cnStr))
            {
                cn.Execute(@"
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
)");
                string json = GetJsonContent("https://travel.taipei/api/zh-tw/crowd/spots");
                Spots spots = Newtonsoft.Json.JsonConvert.DeserializeObject<Spots>(json);
                cn.Execute("DELETE FROM Attraction");
                //參數是用@paramName
                var insertScript =
                    "INSERT INTO Attraction VALUES (@id, @name, @district, @level, @nlat, @elong, @cover, @update_time)";
                cn.Execute(insertScript, spots.data);
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
            var request = System.Net.WebRequest.Create(targetURI);
            request.ContentType = "application/json; charset=utf-8";
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
