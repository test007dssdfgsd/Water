using System;

namespace ApiAll.Model
{
    /// <summary>So'rov natijasi (avvalgi MarketLimitAddedInfo o'rniga).</summary>
    public class LimitAddedInfo
    {
        public DateTime beginDate { get; set; }
        public DateTime endDate { get; set; }
        public int updatedUsersCount { get; set; }
    }
}
