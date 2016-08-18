using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class CartTempSNInfo
    {
        public int AccountID { get; set; }
        public int CartTypeID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
