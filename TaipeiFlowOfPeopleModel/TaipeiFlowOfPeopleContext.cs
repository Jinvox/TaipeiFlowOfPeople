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
        #region Static (Business Logic)
        public static void CheckAndInitial(IConfiguration config, string tableName)
        {
            string dbPath = config.GetSection("SQLiteFile").Value;
            string a = System.IO.Path.GetDirectoryName(dbPath);
            if (!System.IO.File.Exists(dbPath))
            {
                using (var context = new TaipeiFlowOfPeopleContext(config))
                {
                    context.RefreshTable<AttractionAddress>();
                    context.RefreshTable<Attraction>();
                    context.RefreshTable<BusStop>();
                    context.RefreshTable<MetroStation>();
                    context.RefreshTable<YouBikeStation>();
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
                        if (tableName == nameof(Attraction))
                            context.RefreshTable<Attraction>();
                        else if (tableName == nameof(BusStop))
                            context.RefreshTable<BusStop>();
                        else if (tableName == nameof(MetroStation))
                            context.RefreshTable<MetroStation>();
                        else if (tableName == nameof(YouBikeStation))
                            context.RefreshTable<YouBikeStation>();
                    }
                }
            }
        }
        #endregion

        #region Database (Busingee Logic)
        public DbSet<AttractionAddress> AttractionAddress { get; set; }
        public DbSet<Attraction> Attraction { get; set; }
        public DbSet<BusStop> BusStop { get; set; }
        public DbSet<StopName> StopName { get; set; }
        public DbSet<StopPosition> StopPosition { get; set; }
        public DbSet<MetroStation> MetroStation { get; set; }
        public DbSet<StationName> StationName { get; set; }
        public DbSet<StationPosition> StationPosition { get; set; }
        public DbSet<YouBikeStation> YouBikeStation { get; set; }

        public void RefreshTable<T>()
        {
            string dbPath = this.config.GetSection("SQLiteFile").Value;

            if (typeof(Attraction).IsAssignableFrom(typeof(T)))
            {
                string json = CommunityHelper.Instance.GetJsonContent("https://travel.taipei/api/zh-tw/crowd/spots", false);
                Spots entity = Newtonsoft.Json.JsonConvert.DeserializeObject<Spots>(json);
                this.ClearTable<T>();

                // 補上地址
                List<AttractionAddress> addresses = this.Set<AttractionAddress>().ToList();
                foreach (Attraction attriction in entity.data)
                {
                    attriction.address = addresses.FirstOrDefault(x => x.name == attriction.name?.Split('(').FirstOrDefault())?.address;
                }

                this.Set<Attraction>().AddRange(entity.data);
                this.SaveChanges();
            }
            else if (typeof(AttractionAddress).IsAssignableFrom(typeof(T)))
            {
                List<AttractionAddress> entities = new List<AttractionAddress>();
                entities.Add(new AttractionAddress("貓空", "臺北市文山區新光路二段8號"));
                entities.Add(new AttractionAddress("西門町商圈", "台北市萬華區成都路1號"));
                entities.Add(new AttractionAddress("士林官邸公園", "台北市士林區福林路60號"));
                entities.Add(new AttractionAddress("士林觀光夜市", "台北市士林區基河路101號"));
                entities.Add(new AttractionAddress("信義商圈", "台北市信義區松壽路9號"));
                entities.Add(new AttractionAddress("白石湖", "臺北市內湖區碧山路38號"));
                entities.Add(new AttractionAddress("竹子湖", "台北市北投區竹子湖52號"));
                entities.Add(new AttractionAddress("中正紀念堂", "台北市中正區中山南路21號"));
                entities.Add(new AttractionAddress("華山文創園區", "台北市中正區八德路一段1號"));
                entities.Add(new AttractionAddress("松山文創園區", "台北市信義區光復南路133號"));
                entities.Add(new AttractionAddress("陽明公園", "台北市北投區湖山路二段26號"));

                List<AttractionAddress> existedEntities = this.AttractionAddress.ToList();
                List<AttractionAddress> addedEntities = entities.Where(x => !existedEntities.Exists(y => y.name == x.name)).ToList();

                this.Set<AttractionAddress>().AddRange(addedEntities);
                this.SaveChanges();
            }
            else if (typeof(BusStop).IsAssignableFrom(typeof(T)))
            {
                string json = CommunityHelper.Instance.GetJsonContent("https://ptx.transportdata.tw/MOTC/v2/Bus/Stop/City/Taipei?$$format=JSON", true);
                List<BusStop> entities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BusStop>>(json);
                foreach (BusStop entity in entities)
                {
                    entity.StopName.Uid = entity.StopUID;
                    entity.StopPosition.Uid = entity.StopUID;
                }
                this.ClearTable<T>();

                this.Set<BusStop>().AddRange(entities);
                this.SaveChanges();
            }
            else if (typeof(MetroStation).IsAssignableFrom(typeof(T)))
            {
                string json = CommunityHelper.Instance.GetJsonContent("https://ptx.transportdata.tw/MOTC/v2/Rail/Metro/Station/TRTC?$format=JSON", true);
                List<MetroStation> entities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MetroStation>>(json);
                foreach (MetroStation entity in entities)
                {
                    entity.StationName.Uid = entity.StationUID;
                    entity.StationPosition.Uid = entity.StationUID;
                }
                this.ClearTable<T>();

                this.Set<MetroStation>().AddRange(entities);
                this.SaveChanges();
            }
            else if (typeof(YouBikeStation).IsAssignableFrom(typeof(T)))
            {
                string json = CommunityHelper.Instance.GetJsonContentYouBike("https://tcgbusfs.blob.core.windows.net/blobyoubike/YouBikeTP.json");
                YouBikeTP youBikeTP = Newtonsoft.Json.JsonConvert.DeserializeObject<YouBikeTP>(json);
                foreach (YouBikeStation entity in youBikeTP.retVal)
                {
                    if (float.TryParse(entity.lat, out float lat))
                    {
                        entity.latPosition = lat;
                    }
                    if (float.TryParse(entity.lng, out float lng))
                    {
                        entity.lngPosition = lng;
                    }
                }
                this.ClearTable<T>();

                this.Set<YouBikeStation>().AddRange(youBikeTP.retVal);
                this.SaveChanges();
            }
        }
        #endregion

        #region property
        private static bool isMigrated = false;
        private IConfiguration config;
        #endregion

        #region constructor
        public TaipeiFlowOfPeopleContext(IConfiguration configuration)
        {
            config = configuration;
            if (!isMigrated)
            {
                string dbPath = config.GetSection("SQLiteFile").Value;
                string dbDir = System.IO.Path.GetDirectoryName(dbPath);
                if (!System.IO.Directory.Exists(dbDir))
                {
                    System.IO.Directory.CreateDirectory(dbDir);
                }
                bool isExist = System.IO.File.Exists(dbPath);

                this.Database.EnsureCreated();

                if(!isExist)
                {
                    this.RefreshTable<AttractionAddress>();
                    this.RefreshTable<Attraction>();
                    this.RefreshTable<BusStop>();
                    this.RefreshTable<MetroStation>();
                    this.RefreshTable<YouBikeStation>();
                }

                isMigrated = true;
            }
        }
        #endregion

        #region override
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = config.GetSection("SQLiteFile").Value;
            options.UseSqlite($"data source={dbPath}");
        }
        #endregion

        #region method

        public void ClearTable<T>()
        {
            string dbPath = config.GetSection("SQLiteFile").Value;
            string tableName = typeof(T).Name;
            this.Database.ExecuteSqlRaw($"DELETE FROM {tableName}");
        }

        public void CheckAndInitial(string tableName)
        {
            string dbPath = config.GetSection("SQLiteFile").Value;
            string a = System.IO.Path.GetDirectoryName(dbPath);
            if (!System.IO.File.Exists(dbPath))
            {
                this.RefreshTable<Attraction>();
                this.RefreshTable<BusStop>();
                this.RefreshTable<MetroStation>();
                this.RefreshTable<YouBikeStation>();
                return;
            }

            using (var cn = new SqliteConnection($"data source={dbPath}"))
            {
                var table = cn.QueryFirstOrDefault($"SELECT * FROM sqlite_master WHERE type = 'table' and name = '{tableName}'");
                if (table == null)
                {
                    if (tableName == nameof(Attraction))
                        this.RefreshTable<Attraction>();
                    else if (tableName == nameof(BusStop))
                        this.RefreshTable<BusStop>();
                    else if (tableName == nameof(MetroStation))
                        this.RefreshTable<MetroStation>();
                    else if (tableName == nameof(YouBikeStation))
                        this.RefreshTable<YouBikeStation>();
                }
            }
        }
        #endregion
    }
}
