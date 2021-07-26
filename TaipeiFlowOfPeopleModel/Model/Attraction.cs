using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeopleModel
{
    public class Attraction
    {
        [Key]
        public int? id { get; set; }
        
        [MaxLength(50)]
        public string name { get; set; }

        [MaxLength(50)]
        public string district { get; set; }

        public int? level { get; set; }

        public float? nlat { get; set; }

        public float? elong { get; set; }
        [MaxLength(50)]
        public string cover { get; set; }

        [MaxLength(50)]
        public string update_time { get; set; }

        [MaxLength(500)]
        public string address { get; set; }
    }

}
