using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAll.Model.water
{
    [Table("water_tuman")]
    public class WaterTuman : WaterBaseModel
    {
        public String name { get; set; }
        public long WaterViloyatid {get;set;}
        public WaterViloyat viloyat { get; set; }
    
        public long? company_id { get; set; }

        [ForeignKey("company_id")]
        public WaterCompany Company { get; set; }
    }
}
