using System;
//using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Website.ECWeb.Models;
using System.Data;
using System.Data.SqlClient;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.DB;

namespace TWNewEgg.Website.ECWeb.Controllers
{
    public class WebATMController : Controller
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private TWSqlDBContext db = new TWSqlDBContext();

        public CathayUnitedBank WebATMConfirm(CathayUnitedBank WATM)
        {
            //填入國泰世華的訂單資料
            WATM.orderNoGenDate = DateTime.Now.ToString("yyyy/MM/dd");
            int amt = (int)WATM.amount;
            WATM.PtrAcno = AccCashCheck(WATM.salesorder_code, amt);

            /*測試用的資料
            WATM.PtrAcno = "86620610000303";
            WATM.ItemNo = "萬寶龍紅鋼珠筆";
            WATM.amount = 113491;
            WATM.CompanyID = "016790001000";
            WATM.orderNoGenDate = "2005/07/01";
            */

            return WATM;
        }

        //國泰世華驗證碼
        public string AccCashCheck(string salesorder_code, long pricesum)
        {
            string new_pricesum = Convert.ToString(pricesum);
            string PaymentDeadline = salesorder_code.Substring(salesorder_code.Length - 10, 4);  //訂單當天編號
            string OrderNumber = salesorder_code.Substring(salesorder_code.Length - 6, 6);
            string new_testaccount = "6193" + PaymentDeadline + "0" + OrderNumber;

            int CashCount = 0;
            string tt2 = "";
            int Account_weights = 0; // 帳號權數結果
            int Dollar_weighted = 0; // 金額加權結果
            string CheckCode = ""; // 檢查碼
            Account_weights = 0; Dollar_weighted = 0; CheckCode = ""; // 歸0

            CashCount = new_pricesum.Length;

            int init = 4;
            char[] tmpta = new_testaccount.ToCharArray();
            for (int i = 0; i < 15; i++)
            {
                if ((init + i) % 10 == 0)
                    init++;
                Account_weights += ((tmpta[i] - 48) * ((init + i) % 10)) % 10;
            }
            Account_weights = Account_weights % 10; // 帳號權數結果

            for (int i = 0; i < CashCount; i++)
            {
                tt2 += Convert.ToString((i + 1) % 10);
            }
            char[] rett2 = tt2.ToCharArray();
            Array.Reverse(rett2);
            tt2 = new string(rett2); // testcash的權數
            char[] tmptc = new_pricesum.ToCharArray();
            for (int i = 0; i < CashCount; i++)
            {
                Dollar_weighted += ((tmptc[i] - 48) * (tt2[i] - 48)) % 10;
            }
            Dollar_weighted = Dollar_weighted % 10;
            if ((Account_weights + Dollar_weighted) % 10 == 0)
            {
                CheckCode = "0";
            }
            else
            {
                CheckCode = Convert.ToString(10 - ((Account_weights + Dollar_weighted) % 10)); // 檢查碼
            }
            //m3 = Convert.ToString(10 - (m + m2)); // 檢查碼
            string new_code = new_testaccount + CheckCode;

            return new_code;
        }

        //Step2
        //www.xxx.com.tw/WebATM/FISC2Confirm
        //國泰世華確認訂單所需資料(使用者看不到)
        public ActionResult FISC2Confirm(string CompanyID, string OrderNoGenDate, string MerchantKey, string PurQuantity, string PtrAcno = "error", int amt = 0)
        {
            DateTime date = DateTime.Now;
            //寫Log檔
            /*StreamWriter sw = null;
            if (!System.IO.Directory.Exists(this.Server.MapPath("//") + "\\log\\MyBank\\"))
                System.IO.Directory.CreateDirectory(this.Server.MapPath("//") + "\\log\\MyBank\\");
            sw = new StreamWriter(this.Server.MapPath("~/log/MyBank") + "/MyBank_log.txt", true);
            string Step1 = "\r\n第二步FISC2Confirm";
            
            string log = "\r\n接收資料-CompanyID : " + CompanyID + "\r\nOrderNoGenDate : " + OrderNoGenDate + "\r\nMerchantKey : " + MerchantKey + "\r\nPurQuantity : " + PurQuantity + "\r\nPtrAcno : " + PtrAcno + "\r\namt : " + amt;

            sw.WriteLine(Step1);
            sw.WriteLine(date);
            sw.WriteLine(log);*/

            CathayUnitedBank WATM = new CathayUnitedBank();

            //錯誤資訊
            if (PtrAcno == "error")
            {
                /*string log2 = "\r\nPtrAcno沒收到資料";

                sw.WriteLine(log2);
                sw.Close();
                WATM.rtnCode = "9999";
                return PartialView(WATM);*/
            }

            //填入確認資料

            WATM.CompanyID = CompanyID;
            WATM.orderNoGenDate = System.Web.HttpUtility.UrlDecode(OrderNoGenDate);
            WATM.PtrAcno = PtrAcno;
            WATM.MerchantKey = MerchantKey;
            WATM.amount = amt;

            string orderNoGenDate = DateTime.Now.ToString("yyyy/MM/dd");
            if (date.Month < 10)
            {
                WATM.salesorder_code = "LBO" + date.ToString("yyyy").Substring(2) + '0' + date.Month + date.Day + WATM.PtrAcno.Substring(9, 6);

            }
            else
            {
                WATM.salesorder_code = "LBO" + date.ToString("yyyy").Substring(2) + date.Month + date.Day + WATM.PtrAcno.Substring(9, 6);
            }
            string log3;
            SalesOrder SO = new SalesOrder();
            SO = db.SalesOrder.Where(x => x.Code == WATM.salesorder_code).FirstOrDefault();
            if (SO != null)
            {
                if (db.SalesOrderGroup.Where(x => x.ID == SO.SalesOrderGroupID).Any())
                {
                    SalesOrderGroup SOG = new SalesOrderGroup();
                    SOG = db.SalesOrderGroup.Where(x => x.ID == SO.SalesOrderGroupID).FirstOrDefault();
                    if ((WATM.CompanyID == "010230101000") && (WATM.orderNoGenDate == OrderNoGenDate) && (WATM.PtrAcno == AccCashCheck(WATM.salesorder_code, amt)) && (WATM.amount == SOG.PriceSum))
                    {
                        WATM.rtnCode = "0000";
                        /*log3 = "\r\n正式環境確認送出資料-CompanyID : " + WATM.CompanyID + "\r\nOrderNoGenDate : " + WATM.orderNoGenDate + "\r\nPtrAcno : " + WATM.PtrAcno + "\r\nMerchantKey : " + WATM.MerchantKey + "\r\nPurQuantity : " + WATM.PurQuantity + "\r\nrtnCode : " + WATM.rtnCode;
                        sw.WriteLine(log3);
                        sw.Close();*/
                        return PartialView(WATM);
                    }
                    else
                    {
                        WATM.rtnCode = "9999";
                        /*log3 = "\r\n測試環境確認送出資料-CompanyID : " + WATM.CompanyID + "\r\nOrderNoGenDate : " + WATM.orderNoGenDate + "\r\nPtrAcno : " + WATM.PtrAcno + "\r\nMerchantKey : " + WATM.MerchantKey + "\r\nPurQuantity : " + WATM.PurQuantity + "\r\nrtnCode : " + WATM.rtnCode;
                        sw.WriteLine(log3);
                        sw.Close();*/
                        return PartialView(WATM);
                    }
                }
            }
            /*log3 = "與資料庫比對error";
            sw.WriteLine(log3);
            sw.Close();*/

            return PartialView(WATM);
        }

        //Step3
        //www.xxx.com.tw/WebATM/Confirm
        //國泰世華傳入最後結果(訂單成功 or 失敗)，寫入Auth table
        public void Confirm(string CompanyID, string OrderNoGenDate, string OrderNumber, string AcqDate, string AcqTime, string MerchantKey, string TrsCode = "NULL", int amt = 0)
        {
            try
            {
                DateTime date = DateTime.Now;

                //寫Log檔
                /*StreamWriter sw = null;
                sw = new StreamWriter(this.Server.MapPath("~/log/MyBank") + "/MyBank_log.txt", true);
                string Step1 = "\r\n第三步Confirm";
                
                string log = "\r\nCompanyID : " + CompanyID + "\r\nOrderNoGenDate : " + OrderNoGenDate + "\r\nMerchantKey : " + MerchantKey + "\r\nOrderNumber : " + OrderNumber + "\r\nAcqDate : " + AcqDate + "\r\nAcqTime : " + AcqTime + "\r\nTrsCode : " + TrsCode;

                sw.WriteLine(Step1);
                sw.WriteLine(date);
                sw.WriteLine(log);*/

                //寫入資料庫
                string salesorder_code;
                if (date.Month < 10)
                {
                    salesorder_code = "LBO" + date.ToString("yyyy").Substring(2) + '0' + date.Month + date.Day + OrderNumber.Substring(9, 6);
                }
                else
                {
                    salesorder_code = "LBO" + date.ToString("yyyy").Substring(2) + date.Month + date.Day + OrderNumber.Substring(9, 6);
                }
                decimal p = 0;

                SalesOrder send2 = new SalesOrder();
                List<SalesOrderItem> send = new List<SalesOrderItem>();
                List<SalesOrder> ss = new List<SalesOrder>();
                SalesOrder send1 = new SalesOrder();

                send2 = db.SalesOrder.Where(x => x.Code == salesorder_code).FirstOrDefault<SalesOrder>();
                ss = db.SalesOrder.Where(x1 => x1.SalesOrderGroupID == send2.SalesOrderGroupID).ToList<SalesOrder>();

                int nAuthCount = 0;
                foreach (SalesOrder a2 in ss)
                {
                    send = db.SalesOrderItem.Where(x => x.SalesorderCode == a2.Code).ToList<SalesOrderItem>();
                    foreach (SalesOrderItem a1 in send)
                    {
                        p += a1.Price;
                    }
                }
                foreach (SalesOrder a2 in ss)
                {
                    send = db.SalesOrderItem.Where(x => x.SalesorderCode == a2.Code).ToList<SalesOrderItem>();
                    foreach (SalesOrderItem a1 in send)
                    {
                        Auth Auth = new Auth();

                        //看stored procedure撈的資料
                        Auth.SalesOrderItemCode = salesorder_code;

                        //判定是否成功
                        //TrsCode
                        //      9999：(測試)連線成功回應
                        //      0000：(正式)扣款成功回應
                        //      H003：交易結果不確定
                        //      其他皆為失敗              

                        if (TrsCode == "0000")
                        {
                            //正式環境交易成功
                            Auth.SuccessFlag = "1";
                        }
                        else
                        {
                            //交易失敗
                            Auth.SuccessFlag = "0";
                        }

                        //國泰世華 013
                        Auth.AcqBank = "013";
                        Auth.CustomerID = "";
                        Auth.AgreementID = "";
                        try
                        {
                            Auth.SalesOrderGroupID = (int)a2.SalesOrderGroupID;
                        }
                        catch
                        {
                            Auth.SalesOrderGroupID = 0;
                        }
                        send1 = db.SalesOrder.Where(x1 => x1.Code == a2.Code).FirstOrDefault<SalesOrder>();

                        Auth.AccountID = send1.AccountID;

                        //國泰世華虛擬碼
                        Auth.OrderNO = send1.Code;
                        Auth.Qty = (int)a1.Qty;
                        SalesOrderGroup price = db.SalesOrderGroup.Where(x => x.ID == send2.SalesOrderGroupID).FirstOrDefault<SalesOrderGroup>();
                        Auth.Amount = price.PriceSum;
                        Auth.AmountSelf = 0;
                        Auth.Bonus = 0;
                        Auth.BonusBLN = (int)a1.RedmBLN;
                        Auth.HpMark = "";
                        Auth.PriceFirst = 0;
                        Auth.PriceOther = 0;

                        //國泰世華授權碼(交易結果回傳狀態)
                        Auth.AuthCode = TrsCode;
                        //合約編號(國泰世華編號 6913)
                        Auth.AuthSN = OrderNumber;
                        //*國泰世華授權日期
                        Auth.AuthDate = DateTime.Now;
                        //*授權回應碼
                        Auth.RspCode = "";
                        //*授權回應訊息
                        Auth.RspMSG = "";
                        Auth.RspOther = "";
                        Auth.CancelDate = DateTime.Now;
                        Auth.CancelRspCode = "";
                        Auth.CancelRspMSG = "";
                        Auth.FaildealUser = "jh3x";
                        Auth.FaildealNote = "";
                        Auth.FaildealDate = DateTime.Now;
                        Auth.EraseDate = DateTime.Now;
                        Auth.CreateUser = "jh3x";
                        Auth.CreateDate = DateTime.Now;
                        Auth.UpdateUser = "jh3x";
                        db.Auth.Add(Auth);

                        //return View("SalesOrderDetail", ViewBag.sd);
                        nAuthCount++;//懶得改程式, 使用Break強制Auth只寫一筆
                        break;//懶得改程式, 使用Break強制Auth只寫一筆
                    }
                    if (nAuthCount > 0)
                        break;  //懶得改程式, 使用Break強制Auth只寫一筆
                }
                db.SaveChanges();

                Service.PlaceOrder po = new PlaceOrder();
                TWNewEgg.ECWeb.Controllers.CartController PTC = new TWNewEgg.ECWeb.Controllers.CartController(System.Web.HttpContext.Current);

                foreach (SalesOrder a2 in ss)
                {
                    try
                    {
                        //如果產生海外切貨DelvType=6的訂單，通知營服主管
                        SpexService.SendMail(a2.Code);

                        string poCode = po.SendPlaceOrder((int)send2.SalesOrderGroupID, a2.Code);
                        a2.Status = 0; //  貨到付款是否要新增不同的status?來跟其他的已付款的status做區隔?
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        a2.Status = (int)SalesOrder.status.付款成功拋單失敗;
                        db.SaveChanges();
                        MyNeweggController MyNewegg = new MyNeweggController();
                        //MyNewegg.resetpost(a2.Code, 4, "交易失敗", "", "", "", true); // 因為會寄信通知工程師此問題，所以取消訂單動作暫時拿掉
                        //找出需要重新拋送訂單的
                        List<string> NeedReSendList = new List<string>();
                        //掃描訂單狀態
                        Nullable<int> groupID = db.SalesOrder.Where(x => x.Code == salesorder_code).Select(x => x.SalesOrderGroupID).FirstOrDefault();
                        if (groupID != null && groupID > 0)
                        {
                            List<SalesOrder> soList = db.SalesOrder.Where(x => x.SalesOrderGroupID == groupID).ToList();
                            NeedReSendList = soList.Where(x => x.Status != 0).Select(x => x.Code).ToList();
                            PTC.SOReSendMail(NeedReSendList); // 寄發訂單失敗通知信
                        }
                        logger.Error(a2.Code + ":" + e.Message + "  _  ");
                        //return RedirectToAction("Shoppingcart", "MarketableCache", new { message = "交易失敗，請重新選購，謝謝!" });
                    }
                }

                //寄送結帳成功 e-mail

                PTC.Mail_WebMessage(salesorder_code);

                /*sw.Close();*/
            }
            catch
            {
                //有錯誤
            }
        }

        //Step4
        //www.xxx.com.tw/WebATM/Query
        //使用者回來畫面
        public ActionResult Query(string OrderNumber, string MerchantKey, string TrsCode = "0")
        {
            CathayUnitedBank Web = new CathayUnitedBank();
            if (TrsCode == "0000")
            {
                string salesorder_code;
                salesorder_code = "LBO" + DateTime.Now.Year.ToString().Substring(2, 2) + OrderNumber.Substring(4, 4) + OrderNumber.Substring(9, 6);

                return RedirectToAction("Results", "cart", new { ordernumber = salesorder_code });
            }
            else if (TrsCode == "")
            {
                return RedirectToAction("Index", "Home");
            }

            Web.TrsCode = TrsCode;
            Web.PtrAcno = OrderNumber;
            Web.MerchantKey = MerchantKey;
            Web.amount = 0;

            return PartialView(Web);
        }
    }
}
