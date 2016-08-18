using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.ItemService.Models;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class PaymentandDelivery
    {
        public Nullable<int> PayTypeID { get; set; }
        public int PayType0rateNum { get; set; }
        public int DeliverType { get; set; }
    }
}