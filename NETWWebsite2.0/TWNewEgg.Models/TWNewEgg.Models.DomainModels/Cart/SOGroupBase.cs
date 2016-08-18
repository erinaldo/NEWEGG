using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class SOGroupBase
    {
        public int ID { get; set; }
        public string Vaccunt { get; set; }
        public int PriceSum { get; set; }
        public int OrderNum { get; set; }
        public string Note { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string Status { get; set; }
    }
}
