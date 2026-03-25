using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ApiAll.Model
{
    public class JsonPaginationModel
    {
        public int items_count { get; set; }
        public int current_page { get; set; }
        public int current_item_count { get; set; }
        public JArray items_list { get; set; }
    }
}
