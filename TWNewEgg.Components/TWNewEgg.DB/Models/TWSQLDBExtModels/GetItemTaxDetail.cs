using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models.ExtModels
{
    public class GetItemTaxDetail
    {

        [Key]
        public int item_id { get; set; }
        public int itemlist_id { get; set; }
        public string pricetaxdetail { get; set; }

    }
}