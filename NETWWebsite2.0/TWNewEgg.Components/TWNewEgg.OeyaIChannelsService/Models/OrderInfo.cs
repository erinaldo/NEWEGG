using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.OeyaIChannelsService.Models
{
    public class OrderInfoOut
    {
        public string SalesOrderItemCode { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
        public string ProductQty { get; set; }
        public string TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public string OeyaInfo { get; set; }
        public string BackCode { get; set; }
        public string InvalidReason { get; set; }
        public string Other { get; set; }
        public string OrderCreateDate { get; set; }
    }
}
