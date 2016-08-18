using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class GroupDiscount
    {
        public int? EventID { get; set; }
        public string EventName { get; set; }
        public string Desc { get; set; }
        public string CSSStyle { get; set; }
        public string Event_Code { get; set; }
        public decimal DiscountPrice { get; set; }
    }
}
