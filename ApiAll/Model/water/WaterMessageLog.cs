using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAll.Model.water
{
    [Table("water_message_log")]
    public class WaterMessageLog : WaterBaseModel
    {
        public String message_str { get; set; }
        public String note { get; set;}
        public String massager_user_name { get; set; }
        public DateTime reg_date_time { get; set; } = DateTime.Now;
    
        public long? company_id { get; set; }

        [ForeignKey("company_id")]
        public WaterCompany Company { get; set; }
    }
}
