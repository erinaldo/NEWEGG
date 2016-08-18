using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TWNewEgg.InternalSendMail.Model.SendMailModel;

namespace TWNewEgg.InternalSendMail.Service
{
    public class RefundMailList
    {
        public void GetRefundMailList(TWNewEgg.DB.TWBACKENDDB.Models.Cart Cart)
        {
            //TWNewEgg.DB.TWBACKENDDB.Models.Cart
            var XMLEXport = new Service.XMLEXport();
            MailList MailListMode = XMLEXport.GenerateXsdFromXml();
            string mailAddress = "";

            //foreach (var typeNum in MailListMode.RefundList.ToList())
            //{
            //    if (typeNum.DelivType == Cart.ShipType.ToString())
            //    {

            //        foreach (var departmentName in typeNum.MailGroup)
            //        {
            //            var tempAddress = MailListMode.MailGroupList.Where(x => x.Name == departmentName).Select(x => x.MailGroup).FirstOrDefault();
            //            if (mailAddress != "")
            //            {
            //                mailAddress = mailAddress + "," + tempAddress;
            //            }
            //            else mailAddress = tempAddress;
            //        }

            //    }
            //}
                    }
    }

}
