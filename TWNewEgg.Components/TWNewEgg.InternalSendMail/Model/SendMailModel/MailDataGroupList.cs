using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class MailDataGroupList
    {
        string ImagesServer = System.Configuration.ConfigurationManager.AppSettings["Images"];
        public MailDataGroupList() {
            this.CustomerMail = new CustomerMail();
            this.SendPMWithGrossMargin = new TWNewEgg.InternalSendMail.Model.SendMailModel.SendPMWithGrossMargin();
            this.Header = ImagesServer + "/Themes/2013/img/header.png";
            this.Footer = ImagesServer + "/Themes/2013/img/footer.png";
        }

        public List<TWNewEgg.DB.TWBACKENDDB.Models.Retgood> RetgoodList { get; set; }
        public List<TWNewEgg.DB.TWBACKENDDB.Models.refund2c> Refund2cList { get; set; }
        public List<TWNewEgg.DB.TWSQLDB.Models.SalesOrderCancel> SalesOrderCancelList { get; set; }
        public List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList { get; set; }
        public List<TWNewEgg.DB.TWBACKENDDB.Models.Process> ProcessList { get; set; }
        public List<TWNewEgg.DB.TWSQLDB.Models.SalesOrder> SalesOrderList { get; set; }
        public List<TWNewEgg.InternalSendMail.Model.SendMailModel.ItemInStockDetailWithWMS> ItemInStockDetailWithWMS { get; set; }      
        public List<TWNewEgg.DB.TWSQLDB.Models.ItemWarranty> ItemWarranty { get; set; }
        public TWNewEgg.InternalSendMail.Model.SendMailModel.SendPMWithGrossMargin SendPMWithGrossMargin { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public string Exception { get; set; }
        public CustomerMail CustomerMail { get; set; }
        public Dictionary<string, string> DIC { get; set; }
    }
}
