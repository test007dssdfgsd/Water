using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAll.Model
{
    public class PaginationModel
    {
        public List<Users> usersList { get; set; }
        public int count { get; set; }
    }
}
