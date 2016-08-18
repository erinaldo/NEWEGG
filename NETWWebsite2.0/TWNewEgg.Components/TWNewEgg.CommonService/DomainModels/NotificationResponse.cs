using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CommonService.DomainModels
{
    public class NotificationResponse
    {
        public string  ReturnCode { set; get; }
        public string Description { set; get; }
        public string ErrorMessage { set; get; }
    }
}
