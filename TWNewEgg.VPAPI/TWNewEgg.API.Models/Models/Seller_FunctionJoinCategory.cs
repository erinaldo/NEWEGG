using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;

namespace TWNewEgg.API.Models
{
    public class Seller_FunctionJoinCategory : EDI_Seller_Function
    {
        public string CategoryName { get; set; }
    }
}
