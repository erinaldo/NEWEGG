using System;

namespace TWNewEgg.Models.DomainModels.Lottery
{
    public class Game
    {
        public string Description { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
    }
}
