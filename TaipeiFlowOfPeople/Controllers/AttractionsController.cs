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
using System.Threading.Tasks;
using TaipeiFlowOfPeople.Helper;
using TaipeiFlowOfPeopleModel;

namespace TaipeiFlowOfPeople.Controllers
{
    /// <summary>
    /// 景點人潮
    /// </summary>
    [Authorize]
    [EnableCors("Policy1")]
    [Route("api/[controller]")]
    [ApiController]
    public class AttractionsController : ControllerBase
    {
        private IConfiguration config { get; set; }
        private TaipeiFlowOfPeopleContext context { get; set; }
        private JwtHelpers jwt { get; set; }
        public AttractionsController(IConfiguration configuration, TaipeiFlowOfPeopleContext taipeiFlowOfPeopleContext, JwtHelpers jwt)
        {
            this.config = configuration;
            this.context = taipeiFlowOfPeopleContext;
            this.jwt = jwt;
        }

        /// <summary>
        /// 取得所有景點人潮資料最多200筆
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Attraction> Get()
        {
            var entities = context.Attraction.Take(200).ToList();
            return entities;
        }

        /// <summary>
        /// 取得指定編號的景點人朝資料
        /// </summary>
        /// <param name="id">唯一識別編號</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Attraction Get(int id)
        {
            var entity = context.Attraction.Where(x => x.id == id).FirstOrDefault();
            return entity;
        }

        /// <summary>
        /// 以名稱或行政區模糊搜索景點
        /// </summary>
        /// <param name="nameOrDistrict">名稱或行政區</param>
        /// <returns></returns>
        [HttpGet("byNameDistrict")]
        public IEnumerable<Attraction> Get(string nameOrDistrict)
        {
            var entities = context.Attraction.Where(x=>x.name.Contains(nameOrDistrict) || x.district.Contains(nameOrDistrict)).Take(200).ToList();
            return entities;
        }

        /// <summary>
        /// 以名稱過濾景點
        /// </summary>
        /// <param name="name">名稱</param>
        /// <returns></returns>
        [HttpGet("byName")]
        public IEnumerable<Attraction> GetByName(string name)
        {
            var entities = context.Attraction.Where(x => x.name == name).Take(200).ToList();
            return entities;
        }

        /// <summary>
        /// 以行政區過濾景點
        /// </summary>
        /// <param name="district">行政區</param>
        /// <returns></returns>
        [HttpGet("byDistrict")]
        public IEnumerable<Attraction> GetByDistrict(string district)
        {
            var entities = context.Attraction.Where(x => x.district == district).Take(200).ToList();
            return entities;
        }

        /// <summary>
        /// 以人潮等級過濾景點
        /// </summary>
        /// <param name="level">人潮等級</param>
        /// <returns></returns>
        [HttpGet("byLevel")]
        public IEnumerable<Attraction> GetByLevel(int level)
        {
            var entities = context.Attraction.Where(x => x.level == level).Take(200).ToList();
            return entities;
        }

        /// <summary>
        /// 以行政區+人潮等級過濾景點
        /// </summary>
        /// <param name="districts">行政區</param>
        /// <param name="levels">人潮等級</param>
        /// <returns></returns>
        [HttpGet("byDistrictsLevels")]
        public IEnumerable<Attraction> GetByDistrictsLevels(string districts, string levels)
        {
            // 防呆
            string[] ds = districts?.Split(",");
            if (ds == null)
                ds = new string[] { };
            ds = ds.Select(x => x.Trim()).ToArray();

            string[] templs = levels?.Split(",");
            if (templs == null)
                templs = new string[] { };
            List<int?> ls = new List<int?>();
            foreach(string templ in templs)
            {
                if(int.TryParse(templ, out int l))
                {
                    ls.Add(l);
                }
            }

            // 取數據
            var entities = context.Attraction.Where(x => 
                (!(ls.Count() > 0) || ls.Contains(x.level)) &&
                (!(ds.Count() > 0) || ds.Contains(x.district)))
                .Take(200).ToList();
            return entities;
        }

        //[BindProperties]
        //public class GetRequestParameters
        //{
        //    [BindProperty]
        //    public string[] Districts { get; set; }
        //    [BindProperty]
        //    public int[] Levels { get; set; }
        //}

        /// <summary>
        /// 取得 taken
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetToken")]
        public string GetToken()
        {
            string token = jwt.GenerateToken("taipei");
            return token;
        }

        ///// <summary>
        ///// 測試 JWT
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[HttpGet("TestJWT")]
        //public IEnumerable<Attraction> TestJWT()
        //{
        //    var entities = context.Attraction.Take(200).ToList();
        //    return entities;
        //}
    }
}
