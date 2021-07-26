using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaipeiFlowOfPeopleModel
{
    public class AttractionAddress
    {
        public AttractionAddress(string name, string address)
        {
            this.name = name;
            this.address = address;
        }

        [Key]
        [MaxLength(50)]
        public string name { get; set; }

        [MaxLength(500)]
        public string address { get; set; }
    }

}
