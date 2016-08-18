using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class SendPMWithGrossMargin
    {
        string ImagesServer = System.Configuration.ConfigurationManager.AppSettings["Images"];

        public List<string> PMEmail { get; set; }
        public int CategoryID { get; set; }
        public int SellerID { get; set; }
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }

    }
}
