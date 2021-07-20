using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeople.Model
{
    public class Attraction
    {
        public int id { get; set; }
        public string name { get; set; }
        public string district { get; set; }
        public int level { get; set; }
        public float nlat { get; set; }
        public float elong { get; set; }
        public string cover { get; set; }
        public string update_time { get; set; }
    }

}
