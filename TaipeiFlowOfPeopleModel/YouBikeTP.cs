using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeople.Model
{
    public class YouBikeTP
    {
        public int retCode { get; set; }
        public List<YouBikeStation> retVal { get; set; }
    }
}
