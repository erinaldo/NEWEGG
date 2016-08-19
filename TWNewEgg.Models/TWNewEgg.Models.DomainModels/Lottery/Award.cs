using System;

namespace TWNewEgg.Models.DomainModels.Lottery
{
    public class Award
    {
        public string Description { get; set; }
        public int ID { get; set; }
        public int LotteryID { get; set; }
        public string Name { get; set; }
        public int Probability { get; set; }
        public int ShowOrder { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
    }
}
