using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace TaipeiFlowOfPeopleModel
{
    public class TaipeiFlowOfPeopleContext : DbContext
    {
        public static void CheckAndInitial(IConfiguration config, string tableName)
        {
            string dbPath = config.GetSection("SQLiteFile").Value;
            string a = System.IO.Path.GetDirectoryName(dbPath);
            if(!System.IO.File.Exists(dbPath))
            {
                using(var context = new TaipeiFlowOfPeopleContext(config))
                {
                    context.RefreshTable<Attraction>();
                }
                return;
            }

            using (var cn = new SqliteConnection($"data source={dbPath}"))
            {
                var table = cn.QueryFirstOrDefault($"SELECT * FROM sqlite_master WHERE type = 'table' and name = '{tableName}'");
                if (table == null)
                {
                    using (var context = new TaipeiFlowOfPeopleContext(config))
                    {
                        context.RefreshTable<Attraction>();
                    }
                }
            }
        }

        private static bool isMigrated =  false ;
        private IConfiguration config;
        public TaipeiFlowOfPeopleContext(IConfiguration configuration)
        {
            if (!isMigrated)
            {
                config = configuration;
                string dbPath = config.GetSection("SQLiteFile").Value;
                string dbDir = System.IO.Path.GetDirectoryName(dbPath);
                if (!System.IO.Directory.Exists(dbDir))
                {
                    System.IO.Directory.CreateDirectory(dbDir);
                }
                this.Database.EnsureCreated();
                isMigrated = true;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = config.GetSection("SQLiteFile").Value;
            options.UseSqlite($"data source={dbPath}");
        }
        public DbSet<Attraction> Attraction { get; set; }

        public void RefreshTable<T>()
        {
            string dbPath = this.config.GetSection("SQLiteFile").Value;

            if (typeof(Attraction).IsAssignableFrom(typeof(T)))
            {
                string json = CommunityHelper.Instance.GetJsonContent("https://travel.taipei/api/zh-tw/crowd/spots");
                Spots spots = Newtonsoft.Json.JsonConvert.DeserializeObject<Spots>(json);
                this.ClearTable<T>(config);

                this.Set<Attraction>().AddRange(spots.data);
                this.SaveChanges();
            }
        }

        public void ClearTable<T>(IConfiguration config)
        {
            string dbPath = config.GetSection("SQLiteFile").Value;
            string tableName = typeof(T).Name;
            this.Database.ExecuteSqlRaw($"DELETE FROM {tableName}");
            //using (var cn = new SqliteConnection($"data source={dbPath}"))
            //{
            //    cn.Execute($"DELETE FROM {typeof(T).Name}");
            //}
        }

    }
}
