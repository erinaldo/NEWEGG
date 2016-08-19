using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TWNewEgg.DB.TWSQLDB.Models.ExtModels
{
    public class ItemProperty
    {

        public int ProductID { get; set; }
        public List<ItemPropertiesGroup> ItemPropertiesGrouplist { get; set; }
    }
}
