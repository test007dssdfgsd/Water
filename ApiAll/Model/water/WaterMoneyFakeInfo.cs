using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace ApiAll.Model.water
{
    public class WaterMoneyFakeInfo
    {
        public double? full_money { get; set; }
        public long? otmen_client { get; set; }
        public long? real_client { get; set; }
    
        public long? company_id { get; set; }

        [ForeignKey("company_id")]
        public WaterCompany Company { get; set; }
    }
}
