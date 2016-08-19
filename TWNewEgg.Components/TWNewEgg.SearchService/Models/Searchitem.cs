using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;


namespace TWNewEgg.SearchService.Models
{
    public class Searchitem
    {
        public List<ItemSearch> ItemSearch { get; set; }
        public int? ISnumber { get; set; }
        public List<int> SellOrNot { get; set; }
    }
}
