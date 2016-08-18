using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
     public class MailList
    {
         public List<StatusGroup> RetgoodList { get; set; }
         //public List<StatusGroup> RetgoodStatusList { get; set; }
         //public List<RetgoodStatusGroup> RetgoodStatusList { get; set; }
         public List<StatusGroup> DailyList { get; set; }
         public List<StatusGroup> RefundList { get; set; }
         public List<StatusGroup> CancelList { get; set; }
         public List<StatusGroup> ItemInStocList { get; set; }
         public List<StatusGroup> ItemWarranty { get; set; }
         public List<StatusGroup> TestList { get; set; }
         public List<StatusGroup> DelstatusList { get; set; }
         public List<StatusGroup> LogisticsTriggercatchList { get; set; }         
         public List<Group> MailGroupList { get; set; }
    }
}
