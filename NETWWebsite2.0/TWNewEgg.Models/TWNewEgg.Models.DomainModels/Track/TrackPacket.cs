using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Track
{
    public class TrackPacket
    {
        public string Name { get; set; }
        public List<TrackItem> Items { get; set; }
    }
}
