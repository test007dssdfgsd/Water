using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAll.Model.water
{
    [Table("water_admin_auth")]
    public class WaterAdminAuth : WaterBaseModel
    {
        public String password { get; set; }
        public String login { get; set; }
        public int user_type { get; set; } = 0;
        public WaterAdminUser user { get; set; }
        public long WaterAdminUserid { get; set; }
        public int client_type_info { get; set; } = 0;
    }
}
