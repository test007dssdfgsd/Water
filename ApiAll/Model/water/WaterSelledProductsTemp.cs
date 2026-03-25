using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace ApiAll.Model.water
{
    public class WaterSelledProductsTemp
    {
        public String tovar_nomi { get; set; } = "";
        public double? soni { get; set; } = 0.0;
        public double? xaqiqiy_soni { get; set; } = 0.0;
        public double? money_sum { get; set; } = 0.0;
    
        public long? company_id { get; set; }

        [ForeignKey("company_id")]
        public WaterCompany Company { get; set; }
    }
}
