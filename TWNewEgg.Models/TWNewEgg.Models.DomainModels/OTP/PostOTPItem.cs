using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.OTP
{
    public class PostOTPItem
    {  
        //總價
        public int PriceSum { get; set; }
        public int PayType { get; set; }
        public string Moblie { get; set; }
        public int ItemID { get; set; }

    }
}
