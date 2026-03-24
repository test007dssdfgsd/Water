using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAll.Model.water
{
    [Table("water_company")]
    public class WaterCompany : WaterBaseModel
    {
        public String name { get; set; }
        public String address { get; set; }
        public String phone_number { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? payment_date { get; set; }
        public double payment_amount { get; set; } = 0.0;
    }
}
