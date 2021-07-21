using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeople.Model
{
    public class StationPosition
    {
        public float PositionLon { get; set; }
        public float PositionLat { get; set; }
        public string GeoHash { get; set; }

    }
}
