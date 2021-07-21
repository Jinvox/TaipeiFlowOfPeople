using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeopleModel
{
    public class MetroStation
    {
        [Key]
        [MaxLength(50)]
        public string StationUID { get; set; }
        [MaxLength(50)]
        public string StationID { get; set; }
        [MaxLength(50)]
        public StationName StationName { get; set; }
        [MaxLength(500)]
        public string StationAddress { get; set; }
        public bool? BikeAllowOnHoliday { get; set; }
        public DateTime? SrcUpdateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? VersionID { get; set; }
        [MaxLength(50)]
        public StationPosition StationPosition { get; set; }
        [MaxLength(50)]
        public string LocationCity { get; set; }
        [MaxLength(10)]
        public string LocationCityCode { get; set; }
        [MaxLength(50)]
        public string LocationTown { get; set; }
        [MaxLength(10)]
        public string LocationTownCode { get; set; }
    }

}
