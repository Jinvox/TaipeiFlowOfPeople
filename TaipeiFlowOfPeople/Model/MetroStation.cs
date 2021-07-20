using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeople.Model
{
    public class MetroStation
    {
        public string StationUID { get; set; }
        public string StationID { get; set; }
        public StationName StationName { get; set; }
        public string StationAddress { get; set; }
        public bool BikeAllowOnHoliday { get; set; }
        public DateTime SrcUpdateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int VersionID { get; set; }
        public StationPosition StationPosition { get; set; }
        public string LocationCity { get; set; }
        public string LocationCityCode { get; set; }
        public string LocationTown { get; set; }
        public string LocationTownCode { get; set; }
    }

}
