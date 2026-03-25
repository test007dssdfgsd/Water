using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace ApiAll.Model.water
{
    public class WaterStatistikaFakeReport
    {
        public String fio { get; set; } = "";
        public String address { get; set; } = "";
        public String tuman_name { get; set; } = "";
        public String last_order_date { get; set; } = "";
        public double? olgan_suv_soni { get; set; } = 0;
        public double? bakalashka_soni1 { get; set; } = 0;
        
    
        public long? company_id { get; set; }

        [ForeignKey("company_id")]
        public WaterCompany Company { get; set; }
    }
}
