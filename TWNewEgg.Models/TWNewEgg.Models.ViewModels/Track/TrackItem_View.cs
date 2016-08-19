using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Track
{
    public class TrackItem_View
    {
        public TrackItem_View()
        {
            this.CreateDate = DateTime.Parse("1990/01/01");
        }

        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemQty { get; set; }
        public int? CategoryID { get; set; }
        public int? CategoryType { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
