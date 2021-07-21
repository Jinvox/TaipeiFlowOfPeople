using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeopleModel
{
    public class StopName
    {
        [Key, MaxLength(50)]
        public string Uid { get; set; }
        [MaxLength(50)]
        public string Zh_tw { get; set; }
        [MaxLength(50)]
        public string En { get; set; }
    }
}
