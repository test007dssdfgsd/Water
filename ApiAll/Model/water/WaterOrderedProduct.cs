using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace ApiAll.Model.water
{
    public class WaterOrderedProduct
    {
        public String product_name { get; set; } = "";
        public double? product_qty { get; set; } = 0.0;
    
        public long? company_id { get; set; }

        [ForeignKey("company_id")]
        public WaterCompany Company { get; set; }
    }
}
