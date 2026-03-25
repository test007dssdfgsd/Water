using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAll.Model.water
{
    public class WaterCheckFakeModel
    {
        public double? summa{ get; set; }
        public double? naqt { get; set; }
        public double? karta { get; set; }
        public double? debit { get; set; }
        public double? rasxod { get; set; }
        public String fio { get; set; }

        public long? company_id { get; set; }

        [ForeignKey("company_id")]
        public WaterCompany Company { get; set; }
    }
}
