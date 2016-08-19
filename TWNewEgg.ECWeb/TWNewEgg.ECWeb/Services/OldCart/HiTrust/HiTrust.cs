using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Drawing;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using System.Data.SqlClient;
using TWNewEgg.DB;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class HiTrust
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="ordernumber"></param>
        /// <param name="amount"></param>
        /// <param name="pan"></param>
        /// <param name="banckid"></param>
        /// <param name="orderdesc"></param>
        /// <param name="expiry"></param>
        /// <param name="E01"></param>
          log4net.ILog logger;
          public class Info
          {
              public string Massage { get; set; }
            
          
          }
          Info info;

          public HiTrust() 
          {
              logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
              info = new Info();
          
          }
        public void checktype(int storeid = 0, string ordernumber = "", decimal amount = 0, string pan = "", string banckid = "", string orderdesc = "", string expiry = "1409", string E01 = "")
        {
            if (banckid == "081") { crediteauth(storeid, ordernumber, amount, pan, banckid, orderdesc, expiry, E01); }
            else if (banckid == "103")
            {

            }
        }
      
        public object[] crediteauth(int storeid = 0, string ordernumber = "", decimal amount = 0, string pan = "", string banckid = "", string orderdesc = "", string expiry = "1409", string E01 = "",string E03="")
        {
            var autoflag = int.Parse(System.Configuration.ConfigurationManager.AppSettings[System.Configuration.ConfigurationManager.AppSettings["Environment"] + "_" + "Depositflag"]);
            XmlDocument xd = new XmlDocument();
            object[] m = new object[7];
            string rp = "";
            string mp = "";
            logger.Info("oType 開始");
            System.Type oType = System.Type.GetTypeFromProgID("HiB2CCom.eB2CCom.1");
            logger.Info("oType ok");
            object tmp = System.Activator.CreateInstance(oType);
            logger.Info("oType  temp ok");
            string path = "~/Configurations/info.xml";
            string ticketno = "";
            string path1 = HttpContext.Current.Server.MapPath(path);
            oType.InvokeMember("ordernumber", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { ordernumber });//訂單編號
            oType.InvokeMember("amount", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { amount });//金額
            oType.InvokeMember("orderdesc", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { orderdesc });//訂單描述
            oType.InvokeMember("ticketno", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { ticketno });

            oType.InvokeMember("depositflag", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { autoflag });
            string configpath = @"D:\Payment\hitrust\" + storeid + ".conf";
            string configpath2 = @"D:\Payment\hitrust\hiServer.conf";
            //string configpath1 = HttpContext.Current.Server.MapPath(configpath);
            //string configpath3 = HttpContext.Current.Server.MapPath(configpath2);
            if(System.IO.File.Exists(configpath))
                logger.Info(" Com+ configpath = true"  );
            else
                logger.Info(" Com+ configpath = false");
            if(System.IO.File.Exists(configpath2))
                logger.Info(" Com+ configpath2 = true");
            else
                logger.Info(" Com+ configpath2 = false");

            oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { configpath });//商家config檔名稱
          
         
           // StreamWriter WriteFile = new StreamWriter(Filename);
            oType.InvokeMember("SerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { configpath2 });//Server conf路徑
            //卡號及有效日期
            oType.InvokeMember("pan", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { pan });//卡號
            oType.InvokeMember("expiry", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { expiry });//效期
            oType.InvokeMember("E01", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { E01 });//末三碼
            if (E03 != "") 
            {

                oType.InvokeMember("E03", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { E03 });//分期
            }
            xd.Load(path1);
            if (xd.ChildNodes.Count == 0)
            { }
            else
            {
                foreach (XmlNode node in xd.ChildNodes)
                {


                    XmlElement xe = node as XmlElement;

                    foreach (XmlNode node3 in xe.ChildNodes)
                    {
                        XmlElement xe1 = node3 as XmlElement;
                        string a = xe1.GetAttribute("ID");
                        if (a == banckid)
                        {
                            if (node3.ChildNodes.Count > 0)
                            {
                                XmlNodeList I = xe.GetElementsByTagName("QueryFlag");
                                XmlNode node8 = I[0].NextSibling;
                                XmlElement xe8 = node8 as XmlElement;
                                XmlNode node5 = xe8.NextSibling;
                                XmlElement xe3 = node5 as XmlElement;
                                mp = xe3.GetAttribute("path");
                                rp = xe8.GetAttribute("path");


                            }
                            //xe.GetElementsByTagName("ReturnURL");
                            //oType.InvokeMember("returnURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { "http://118.163.91.153:8021/hitrust/index/B2C_ReturnTest.aspx" });//return連結
                        }

                    }
                }
            }
            // XmlNodeList nodelist = xd.SelectNodes("//Bank");
            logger.Info("oType ok" + configpath);
            oType.InvokeMember("returnURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { rp });
            oType.InvokeMember("merupdateURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { mp });//update連結//return連結
            oType.InvokeMember("B2CAuth", System.Reflection.BindingFlags.InvokeMethod, null, tmp, null);
            string token = (string)oType.InvokeMember("Token", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            string retcode = (string)oType.InvokeMember("retcode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            string authCode = (string)oType.InvokeMember("authCode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
            //m[0] = token;
            m[0] = retcode;
            m[1] = storeid;
            m[2] = token;
            m[3] = authCode;
            logger.Info("mm" + m[0] + "mm1" + m[1] + "mm2" + m[2]);
            return m;
        }


        public object[] crediteauth1(string mm = "", string odernumber1 = "", int storeid = 0)
        {
            object[] m1 = new object[15];

            System.Type oType = System.Type.GetTypeFromProgID("HiB2CCom.eB2CCom.1");
            logger.Info("oType  temp ok");
            object tmp = System.Activator.CreateInstance(oType);
           

                oType.InvokeMember("ordernumber", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { odernumber1 });//訂單編號
                string configpath = "D:/Payment/hitrust/" + storeid + ".conf";
                string configpath2 = "D:/Payment/hitrust/hiServer.conf";
                //string configpath1 = HttpContext.Current.Server.MapPath(configpath);
                //string configpath3 = HttpContext.Current.Server.MapPath(configpath2);
                oType.InvokeMember("MerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { configpath }); //商家config檔路徑
                oType.InvokeMember("SerConfigName", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { configpath2 });//Server conf路徑

                oType.InvokeMember("B2CQuery", System.Reflection.BindingFlags.InvokeMethod, null, tmp, null);//呼叫查詢函式

                string B2Cretcode = (string)oType.InvokeMember("retcode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);//取得訂單回傳代碼
                string authCode = (string)oType.InvokeMember("authCode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);//授權碼
                string authRRN = (string)oType.InvokeMember("authRRN", System.Reflection.BindingFlags.GetProperty, null, tmp, null);//銀行調單編號
                string depositamount = (string)oType.InvokeMember("depositamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string paybatchnumber = (string)oType.InvokeMember("paybatchnumber", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string capDate = (string)oType.InvokeMember("capDate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string credamount = (string)oType.InvokeMember("credamount", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string credbatchnumber = (string)oType.InvokeMember("credbatchnumber", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string credRRN = (string)oType.InvokeMember("credRRN", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string credCode = (string)oType.InvokeMember("credCode", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string creddate = (string)oType.InvokeMember("creddate", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string acquirer = (string)oType.InvokeMember("acquirer", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string cardtype = (string)oType.InvokeMember("cardtype", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                string eci = (string)oType.InvokeMember("eci", System.Reflection.BindingFlags.GetProperty, null, tmp, null);
                m1[0] = B2Cretcode;
                m1[1] = authRRN;
                m1[2] = depositamount;
                m1[3] = paybatchnumber;
                m1[4] = capDate;
                m1[5] = credamount;
                m1[6] = credbatchnumber;
                m1[7] = credRRN;
                m1[8] = credCode;
                m1[9] = creddate;
                m1[10] = acquirer;
                m1[11] = cardtype;
                m1[12] = eci;
                m1[13] = authCode;
                m1[14] = (string)oType.InvokeMember("orderstatus", System.Reflection.BindingFlags.GetProperty, null, tmp, null);

          
            return m1;
        }


        public object[] case1(List<Models.InsertSalesOrdersBySellerOutput> send, decimal p = 0, string salesorder_cardno = "", string salesorder_cardexpire = "", string auth_code_3 = "",string E03="")
        {
            decimal amount = p * 100;
            string banckid = "081";

            object[] mm1 = new object[7];
            object[] mlist = new object[15];
            
            List<int> ss = send.Select(x => (int)x.salesorder_salesordergroupid).Distinct().ToList();
            TWSqlDBContext db = new TWSqlDBContext();

            foreach (var i in ss)
            {

                var send1 = db.SalesOrder.Where(x => x.SalesOrderGroupID == i).OrderBy(x => x.Code).FirstOrDefault<TWNewEgg.DB.TWSQLDB.Models.SalesOrder>();
                int itmelist = db.SalesOrderItem.Where(x => x.SalesorderCode == send1.Code).Select(x => x.ItemlistID).FirstOrDefault();
                if (itmelist == 0)
                {
                    string odercode = send1.Code.ToString();
                    //XmlDocument xd = new XmlDocument();
                    int storeid = GetStoreid(banckid);

                    // string path = "~/Configurations/info.xml";
                    //string path1 = HttpContext.Current.Server.MapPath(path);
                    // xd.Load(path1);

                    string ordernumber = odercode;
                    string pan = salesorder_cardno;//卡號

                    string orderdesc = "this is test order";


                    string expiry = salesorder_cardexpire;
                    string E01 = auth_code_3;//末三碼

                    mm1 = crediteauth(storeid, ordernumber, amount, pan, banckid, orderdesc, expiry, E01,E03);
                   
                       // mlist = crediteauth1(mm1[0].ToString(), ordernumber, storeid);
                        mm1[4] = ordernumber;
                        mm1[5] = mm1[3];
                        mm1[0] = mm1[0];
                   
                    //else { mm1[0] = mm1[0]; }
                    logger.Info("oType over");
                  
                }

            }
            return mm1;
        }

        public int GetStoreid(string banckid)
        {
            XmlDocument xd = new XmlDocument();
            string path = "~/Configurations/info.xml";
            string path1 = HttpContext.Current.Server.MapPath(path);
            xd.Load(path1);
            string sid = "";

            foreach (XmlNode node in xd.ChildNodes)
            {
                XmlElement xe = node as XmlElement;
                foreach (XmlNode node3 in xe.ChildNodes)
                {
                    XmlElement xe1 = node3 as XmlElement;
                    string a = xe1.GetAttribute("ID");

                    if (a == banckid)
                    {
                        if (node3.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode node4 in node3)
                            {
                                XmlElement xe2 = node4 as XmlElement;
                                sid = xe2.GetAttribute("ID");
                            }
                        }
                        //xe.GetElementsByTagName("ReturnURL");
                        //oType.InvokeMember("returnURL", System.Reflection.BindingFlags.SetProperty, null, tmp, new object[] { "http://118.163.91.153:8021/hitrust/index/B2C_ReturnTest.aspx" });//return連結
                    }

                }
            }
            int storeid = Int32.Parse(sid);
            return storeid;
        }
    }
}