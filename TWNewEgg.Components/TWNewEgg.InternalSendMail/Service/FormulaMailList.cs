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
    public class FormulaMailList
    {
        public string GetMailList(List<TWNewEgg.DB.TWBACKENDDB.Models.Cart> CartList, List<TWNewEgg.DB.TWSQLDB.Models.SalesOrder> SalesOrderList, string MailListName, string AccountMail, int Status)
        {
            string MailAddressList = "";
            if (AccountMail == "" || AccountMail == null)
            {
                var XMLEXport = new Service.XMLEXport();

                List<string> CartShipTypeList = new List<string>();
                MailList MailListMode = XMLEXport.GenerateXsdFromXml();
                if (CartList != null)
                {
                    CartShipTypeList = CartList.GroupBy(x => x.ShipType).Select(x => x.Key.ToString()).ToList();
                    if (CartShipTypeList == null || CartShipTypeList.Count == 0)
                    {
                        if (SalesOrderList != null)
                        {
                        CartShipTypeList = SalesOrderList.GroupBy(x => x.DelivType).Select(x => x.Key.ToString()).ToList();
                        }
                    }
                }
                else if (CartList == null && SalesOrderList != null)
                {
                    CartShipTypeList = SalesOrderList.GroupBy(x => x.DelivType).Select(x => x.Key.ToString()).ToList();
                }
                else
                {
                    CartShipTypeList.Add("0");
                }

                List<string> MailGroupList = new List<string>();
                List<string> MailGroupListName = new List<string>();
                List<Group> MailAddressGroupList = new List<Group>();
                List<DelivGroup> AllDeliv = new List<DelivGroup>();
                List<List<string>> MailGroupListtemp = new List<List<string>>();

                //信件種類
                switch (MailListName)
                {
                    case "Cancel":
                        AllDeliv = MailListMode.CancelList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "InnerCancelSO":
                        AllDeliv = MailListMode.CancelList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "CancelSO":
                        AllDeliv = MailListMode.CancelList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "Refund":
                        AllDeliv = MailListMode.RefundList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "finreturnRefund":
                        AllDeliv = MailListMode.RefundList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "abnormalRefund":
                        AllDeliv = MailListMode.RefundList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "retgood":
                        AllDeliv = MailListMode.RetgoodList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "retgood_CustomerRejection":
                        AllDeliv = MailListMode.RetgoodList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "retgood_DeliveryFailure":
                        AllDeliv = MailListMode.RetgoodList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "abnormalReturn":
                        AllDeliv = MailListMode.RetgoodList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "cancelReturn":
                        AllDeliv = MailListMode.RetgoodList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "completeReturn":
                        AllDeliv = MailListMode.RetgoodList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "DailyReportforCategory":
                        AllDeliv = MailListMode.RetgoodList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "DailyList":
                        AllDeliv = MailListMode.DailyList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "inventoryComparison":
                        AllDeliv = MailListMode.ItemInStocList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "itemWarranty":
                        AllDeliv = MailListMode.ItemWarranty.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "DelstatusList":
                        AllDeliv = MailListMode.DelstatusList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "LogisticsTriggercatch":
                        AllDeliv = MailListMode.LogisticsTriggercatchList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    case "Test":
                        AllDeliv = MailListMode.TestList.Where(x => x.Status == Status).Select(x => x.AllDeliv).FirstOrDefault();
                        break;
                    default:
                        break;
                }

                if (AllDeliv != null)
                {
                    if (CartShipTypeList == null || CartShipTypeList.Count == 0)
                    {

                        MailGroupListtemp = AllDeliv.Select(x => x.MailGroup).ToList();
                        MailGroupListtemp.ForEach(x => MailGroupList.AddRange(x));
                        MailGroupListName = MailGroupList.GroupBy(x => x).Select(x => x.Key).ToList();
                        MailAddressGroupList = MailListMode.MailGroupList.Where(x => MailGroupListName.Contains(x.Name)).ToList();
                    }
                    else
                    {
                        MailGroupListtemp = AllDeliv.Where(x => CartShipTypeList.Contains(x.DelivType)).Select(x => x.MailGroup).ToList();
                        MailGroupListtemp.ForEach(x => MailGroupList.AddRange(x));
                        MailGroupListName = MailGroupList.GroupBy(x => x).Select(x => x.Key).ToList();
                        MailAddressGroupList = MailListMode.MailGroupList.Where(x => MailGroupListName.Contains(x.Name)).ToList();
                    }
                    foreach (var MailAddressGroupListitem in MailAddressGroupList)
                    {
                        if (MailAddressList != "")
                        {
                            MailAddressList = MailAddressList + "," + MailAddressGroupListitem.MailGroup;
                        }
                        else
                        {
                            MailAddressList = MailAddressGroupListitem.MailGroup;
                        }
                    }
                }
            }
            else
            {
                MailAddressList = AccountMail;
            }
            return MailAddressList;
        }

        internal static string GetMailList()
        {
            throw new NotImplementedException();
        }
    }
}
