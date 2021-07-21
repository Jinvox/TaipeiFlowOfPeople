using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeopleModel
{
    public class YouBikeStation
    {
        [Key]
        [MaxLength(50)]
        public string sno { get; set; }
        [MaxLength(50)]
        public string sna { get; set; }
        [MaxLength(50)]
        public string tot { get; set; }
        [MaxLength(50)]
        public string sbi { get; set; }
        [MaxLength(50)]
        public string sarea { get; set; }
        [MaxLength(50)]
        public string mday { get; set; }
        [MaxLength(50)]
        public string lat { get; set; }
        public float? latPosition { get; set; }
        [MaxLength(50)]
        public string lng { get; set; }
        public float? lngPosition { get; set; }
        [MaxLength(50)]
        public string ar { get; set; }
        [MaxLength(50)]
        public string sareaen { get; set; }
        [MaxLength(50)]
        public string snaen { get; set; }
        [MaxLength(50)]
        public string aren { get; set; }
        [MaxLength(50)]
        public string bemp { get; set; }
        [MaxLength(50)]
        public string act { get; set; }
    }
}
