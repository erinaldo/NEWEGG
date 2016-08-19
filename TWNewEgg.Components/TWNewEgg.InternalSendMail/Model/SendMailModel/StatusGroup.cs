using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class StatusGroup
    {
         public string StatusName { get; set; }
         public int Status { get; set; }
         public List<DelivGroup> AllDeliv { get; set; }
    }
}
