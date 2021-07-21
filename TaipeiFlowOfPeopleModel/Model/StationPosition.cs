using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeopleModel
{
    public class StationPosition
    {
        [Key]
        public string Uid { get; set; }

        public float? PositionLon { get; set; }

        public float? PositionLat { get; set; }

        [MaxLength(50)]
        public string GeoHash { get; set; }

    }
}
