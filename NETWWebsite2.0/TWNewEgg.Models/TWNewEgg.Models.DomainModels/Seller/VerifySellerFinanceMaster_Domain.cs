using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TWNewEgg.Models.DomainModels.SellerFinance
{
   public class VerifySellerFinanceMaster_Domain
   {
       // Seller結帳週期類別
       public enum BillingCycleType
       {
           半月結 = 1,
           月結 = 2
       }



       public VerifySellerFinanceMaster_Domain()
       {
           this.EndDate = DateTime.UtcNow.AddHours(8);
           this.StartDate = DateTime.UtcNow.AddHours(8);
        }
       public Nullable<System.DateTime> EndDate { get; set; }
       public Nullable<System.DateTime> StartDate { get; set; }
       public string CreateMoth { get; set; }
       public int? BillingCycle { get; set; }
       public int? sellerID { get; set; }
       public string InUserID { get; set; }

       

    }
}
