using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeopleModel
{
    public class BusStop
    {
        [Key]
        [MaxLength(50)]
        public string StopUID { get; set; }
        [MaxLength(50)]
        public string StopID { get; set; }
        [MaxLength(50)]
        public string AuthorityID { get; set; }
        
        [MaxLength(50)]
        public StopName StopName { get; set;}

        [MaxLength(50)]
        public StopPosition StopPosition { get; set; }

        [MaxLength(500)]
        public string StopAddress { get; set; }

        [MaxLength(50)]
        public string Bearing { get; set; }

        [MaxLength(50)]
        public string StationID { get; set; }

        [MaxLength(200)]
        public string City { get; set; }

        [MaxLength(10)]
        public string CityCode { get; set; }
        [MaxLength(10)]
        public string LocationCityCode { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? VersionID { get; set; }
    }
}
