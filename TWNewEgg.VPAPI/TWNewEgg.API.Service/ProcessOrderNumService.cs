using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using System.Data;
using TWNewEgg.API.Models;
using System.Web;
using log4net;
using log4net.Config;
using System.Threading;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// 訂單處理 Service
    /// </summary>
    public class ProcessOrderNumService
    {
        private enum JudgeQty
        { 
            Unset,
            Under,
            Nonunder
        }

        private enum NotificationTypeCode
        { 
            /// <summary>
            /// Order Notifications
            /// </summary>
            ON,
            /// <summary>
            /// Void Order Notificatin
            /// </summary>
            VON,
            /// <summary>
            /// Business Notification
            /// </summary>
            BN,
            /// <summary>
            /// Financial Notification
            /// </summary>
            FN,
            /// <summary>
            /// RMA Notifications
            /// </summary>
            RMA
        }

        private static ILog log = LogManager.GetLogger(typeof(ProcessOrderNumService));

        private DB.TWSellerPortalDBContext sellerPortaldb = new DB.TWSellerPortalDBContext();
        private DB.TWBackendDBContext backendSqldb = new DB.TWBackendDBContext();
        private DB.TWSqlDBContext frontendSqldb = new DB.TWSqlDBContext();
        // 共用的 Response Body
        private Dictionary<string, string> sendMailResult = new Dictionary<string, string>();

        private string productName = string.Empty;

        private string orderUserEmailList = string.Empty;

        private string temporderNum = string.Empty;

        #region private Function

        /// <summary>
        /// 搜尋信件收件人 by sellerID
        /// </summary>
        /// <param name="sellerID"></param>
        /// <param name="mailAction">搜尋要寄送信件收件人的種類</param>
        /// <returns></returns>
        private string[] SearchSellerEmail(string sellerID, NotificationTypeCode mailAction)
        {
            string strSellerID = sellerID;
            int intSellerID = -1;
            int.TryParse(strSellerID, out intSellerID);
            string action = mailAction.ToString();

            // 搜尋收件人，由 BasicInfo 、 Notification
            var userEmailVAR = (from p in sellerPortaldb.Seller_BasicInfo
                                join q in sellerPortaldb.Seller_Notification.Where(x => x.NotificationTypeCode.ToUpper() == action).Where(y => y.Enabled.ToUpper() == "Y") on p.SellerID equals q.SellerID into r
                                from s in r.DefaultIfEmpty()
                                where
                                    p.SellerID == intSellerID
                                select new { p.SellerEmail, s.EmailAddress1, s.EmailAddress2, s.EmailAddress3 }).FirstOrDefault();

            string userEmailStr;
            string[] userEmailStr_Array;
            if (userEmailVAR != null)
            {
                userEmailStr = userEmailVAR.SellerEmail.ToLower()
                             + (string.IsNullOrEmpty(userEmailVAR.EmailAddress1) ? string.Empty : "," + userEmailVAR.EmailAddress1.ToLower())
                             + (string.IsNullOrEmpty(userEmailVAR.EmailAddress2) ? string.Empty : "," + userEmailVAR.EmailAddress2.ToLower())
                             + (string.IsNullOrEmpty(userEmailVAR.EmailAddress3) ? string.Empty : "," + userEmailVAR.EmailAddress3.ToLower());
                userEmailStr_Array = userEmailStr.Split(',');
            }
            else
            {
                userEmailStr = null;
                userEmailStr_Array = null;
            }

            return userEmailStr_Array;
        }

        /// <summary>
        /// 當信件發生Error ，寄送管理者信件
        /// </summary>
        /// <param name="orderNum"></param>
        /// <param name="productID"></param>
        /// <param name="userMail"></param>
        /// <param name="errorMessage"></param>
        private void sendAdminMail(string orderNum, int? productID, string userMail, string errorMessage)
        {
            API.Models.Connector connector = new Models.Connector();
            string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            Models.Mail mailContent = new Models.Mail();

            string[] errorInfo = adminEmail.Split(',');

            switch (orderNum)
            {
                case "庫存":
                    {
                        mailContent.MailMessage = temporderNum + ", " + "Error: " + errorMessage;
                    }
                    break;
                default:
                    mailContent.MailMessage = orderNum + ", ProductID: " + productID
                    + ", User: " + userMail + ", Time: " + DateTime.Now + ", Error: " + errorMessage;
                    // 找欄位存 OrderNum ，不新開欄位
                    break;
            }

            foreach (var mail in errorInfo)
            {
                mailContent.UserEmail = mail;
                mailContent.MailType = Models.Mail.MailTypeEnum.ErrorInfo;
                Thread.Sleep(1000);
                connector.SendMail(null, null, mailContent);
            }
        }


        #endregion

      
        #region 訂單處理，寄送訂單通知、庫存警示信

        /// <summary>
        /// 接收到訂單，處理Mail
        /// </summary>
        /// <param name="orderNum">訂單編號</param>
        /// <returns>回傳dictionary列表顯示各信件寄送情況</returns>
        public ActionResponse<Dictionary<string, string>> ProcessOrderNumMail(string orderNum)
        {
            ActionResponse<Dictionary<string, string>> processResult = new ActionResponse<Dictionary<string, string>>();
            API.Models.Connector conn = new Connector();
            string website = conn.GetAPIWebConfigSetting("TWSPHost");
            string processVoidEmail = conn.GetAPIWebConfigSetting("VoidProcessEmailAddress");
            var voidemail = processVoidEmail.ToLower().Split(',').ToList();
            
            log.Info("訂單通知 CartID: " + orderNum);
            temporderNum = orderNum;

            var productProcessInfo = this.backendSqldb.Process.Where(x => x.CartID == orderNum).FirstOrDefault();

            if (productProcessInfo == null)
            {
                processResult.Finish(false, (int)ResponseCode.Error, "查無Process資料", null);
                //processResult.IsSuccess = false;
                //processResult.Msg = "查無Process資料";
                //processResult.Code = (int)ResponseCode.Error;
                //processResult.Body = null;
                log.Error("orderNum: " + orderNum + "查無Process資料");
                return processResult;
            }

            var productInfo = this.frontendSqldb.Product.Where(x => x.ID == productProcessInfo.ProductID).FirstOrDefault();

            if (productProcessInfo == null)
            {
                processResult.IsSuccess = false;
                processResult.Msg = "查無Product資料";
                processResult.Code = (int)ResponseCode.Error;
                processResult.Body = null;
                log.Error("orderNum: " + orderNum + "查無Product資料");
                return processResult;
            }

            var userInfo = this.sellerPortaldb.Seller_BasicInfo.Where(y => y.SellerID == productInfo.SellerID).FirstOrDefault();

            if (userInfo == null)
            {
                //查無USER資料 >> 不繼續執行剩餘程式段,回傳錯誤
                processResult.IsSuccess = false;
                processResult.Msg = "查無USER資料";
                processResult.Code = (int)ResponseCode.Error;
                processResult.Body = null;
                log.Error("orderNum: " + orderNum + "查無USER資料");
                return processResult;
            }

            string[] userEmailStr_Array = SearchSellerEmail(userInfo.SellerID.ToString(), NotificationTypeCode.ON);

            switch (website)
            {
                case "http://sellerportal.newegg.com.tw":
                    {
                        if (voidemail.Contains(userInfo.SellerEmail))
                        {
                            log.Info("訂單編號: " + orderNum + ", 屬於台蛋的: " + userInfo.SellerEmail + "未寄送。");

                            processResult.IsSuccess = true;
                            processResult.Msg = "此訂單信箱寄送人屬於不處理清單內，Email: " + userInfo.SellerEmail;
                            processResult.Code = (int)ResponseCode.Success;
                            processResult.Body = null;
                        }
                        else
                        {
                            processResult = SendProcessOrderMail(orderNum, productProcessInfo, userInfo, userEmailStr_Array, NotificationTypeCode.ON);    
                        }                 
                    }

                    break;
                default:
                    {
                        if (voidemail.Contains(userInfo.SellerEmail))
                        {
                            log.Info("訂單編號: " + orderNum + ", 屬於台蛋的: " + userInfo.SellerEmail + "未寄送。");

                            processResult.IsSuccess = true;
                            processResult.Msg = "此訂單信箱寄送人屬於不處理清單內，未寄出。Email: " + userInfo.SellerEmail;
                            processResult.Code = (int)ResponseCode.Success;
                            processResult.Body = null;
                        }
                        else
                        {
                            processResult = TestProcessOrderMail(userEmailStr_Array, userInfo.SellerEmail, NotificationTypeCode.ON);
                        }                       
                    }

                    break;
            }

            return processResult;
        }

        private ActionResponse<Dictionary<string, string>> TestProcessOrderMail(string[] userEmailStr_Array, string inventoryEmail, NotificationTypeCode action)
        {
            ActionResponse<Dictionary<string, string>> processResult = new ActionResponse<Dictionary<string, string>>();

            switch (action)
            {
                case NotificationTypeCode.ON:
                    {
                        processResult = TestOrderInfo(userEmailStr_Array, inventoryEmail);
                    }
                    break;
                case NotificationTypeCode.VON:
                    {
                        processResult = TestVoidOrderInfo(userEmailStr_Array);
                    }
                    break;

            }

            return processResult;
        }

        private ActionResponse<Dictionary<string, string>> TestVoidOrderInfo(string[] userEmailStr_Array)
        {
            ActionResponse<Dictionary<string, string>> processResult = new ActionResponse<Dictionary<string, string>>();

            if (userEmailStr_Array.Count() == 0)
            {
                int emailindex = 1;

                foreach (var email in userEmailStr_Array)
                {
                    this.sendMailResult.Add("訂單取消通知信" + emailindex.ToString(), string.Format("{0} 測試區未寄出。", email));

                    emailindex++;
                }

                processResult.IsSuccess = false;
                processResult.Msg = "測試失敗，有 Usermail 未找到。";
                processResult.Code = (int)ResponseCode.Success;
                processResult.Body = sendMailResult;
            }
            else
            {
                int emailindex = 1;
                foreach (var email in userEmailStr_Array)
                {
                    this.sendMailResult.Add("訂單取消通知信" + emailindex.ToString(), string.Format("{0} 測試區未寄出。", email));

                    emailindex++;
                }

                processResult.IsSuccess = true;
                processResult.Msg = "測試成功";
                processResult.Code = (int)ResponseCode.Success;
                processResult.Body = sendMailResult;
            }

            return processResult;
        }

        private ActionResponse<Dictionary<string, string>> TestOrderInfo(string[] userEmailStr_Array, string inventoryEmail)
        {
            ActionResponse<Dictionary<string, string>> processResult = new ActionResponse<Dictionary<string, string>>();

            if (userEmailStr_Array.Count() == 0 || inventoryEmail == null)
            {
                int emailindex = 1;

                foreach (var email in userEmailStr_Array)
                {
                    this.sendMailResult.Add("新訂單通知信" + emailindex.ToString(), string.Format("{0} 測試區未寄出。", email));

                    emailindex++;
                }

                sendMailResult.Add("庫存警示信", string.Format("商品庫存量小於等於庫存警示量，測試區未寄出。mail:{0}", inventoryEmail));

                processResult.IsSuccess = false;
                processResult.Msg = "測試失敗，有 Usermail 未找到。";
                processResult.Code = (int)ResponseCode.Success;
                processResult.Body = sendMailResult;
            }
            else
            {
                int emailindex = 1;
                foreach (var email in userEmailStr_Array)
                {
                    this.sendMailResult.Add("新訂單通知信" + emailindex.ToString(), string.Format("{0} 測試區未寄出。", email));

                    emailindex++;
                }

                sendMailResult.Add("庫存警示信", string.Format("商品庫存量小於等於庫存警示量，測試區未寄出。mail:{0}", inventoryEmail));

                processResult.IsSuccess = true;
                processResult.Msg = "測試成功";
                processResult.Code = (int)ResponseCode.Success;
                processResult.Body = sendMailResult;
            }

            return processResult;
        }

        /// <summary>
        /// 寄出接收到訂單所要處理的信件
        /// 1. 訂單通知信
        /// 2. 訂單取消通知
        /// 3. 庫存警示信
        /// </summary>
        /// <param name="orderNumber">訂單編號</param>
        /// <param name="productProcessInfo">後台購物車子單資料，供訂單通知搜尋 ProductID 使用</param>
        /// <param name="userInfo">由 ProductID 搜尋商家資訊，得到 SellerID</param>
        /// <param name="userEmailStr_Array">訂單通知所要寄送的 Seller 列表</param>
        /// <param name="notificationType">通知信類型</param>
        /// <returns>回傳 Model Dictionary 列表查詢寄送狀況</returns>
        private ActionResponse<Dictionary<string, string>> SendProcessOrderMail(string orderNumber, DB.TWBACKENDDB.Models.Process productProcessInfo, DB.TWSELLERPORTALDB.Models.Seller_BasicInfo userInfo, string[] userEmailStr_Array, NotificationTypeCode notificationType)
        {
            ActionResponse<Dictionary<string, string>> processResult = new ActionResponse<Dictionary<string, string>>();

            try
            {
                #region 寄送訂單通知信(此部分by 呼叫 Ted 程式)  //20140619 Ted Add 寄送MAIL 通知商家新訂單
                
                #region 移除寄信清單中重覆的信箱地址(避免重覆發信)

                // 將寄信清單 Array 轉為 List
                List<string> list_EmailAddr = new List<string>();
                foreach (string singleMailAddr in userEmailStr_Array)
                {
                    list_EmailAddr.Add(singleMailAddr);
                }

                // 移除 List 中重覆的部份
                List<string> list_EmailAddr_Dictinct = list_EmailAddr.Distinct().ToList();

                #endregion 移除寄信清單中重覆的信箱地址(避免重覆發信)

                // 填寫信件內容
                Models.Mail mailContent = FillMail(notificationType, userInfo, orderNumber);

                // 記錄訊息的抬頭文字
                string logTitle = string.Empty;
                switch (notificationType)
                {
                    case NotificationTypeCode.ON:
                        {
                            logTitle = "新訂單通知信";

                            #region 庫存警示信

                            if (productProcessInfo != null)
                            {
                                this.productName = productProcessInfo.Title;
                            }

                            JudgeQty isUnderSafeQty = new JudgeQty();

                            // 判斷庫存量及是否需要寄送庫存警示信
                            isUnderSafeQty = IsUnderStockSafeQty(productProcessInfo.ProductID);

                            IsSendInventoryMail(isUnderSafeQty, productName, userInfo, list_EmailAddr_Dictinct);

                            #endregion 庫存警示信

                            break;
                        }
                    case NotificationTypeCode.VON:
                        {
                            logTitle = "訂單取消通知信";
                            break;
                        }
                }


                // 收件人計數
                int _mailAddrIndex = 1;
                foreach (string singleMailAddr in list_EmailAddr_Dictinct)
                {
                    // 更換寄送對象
                    mailContent.UserEmail = string.Empty;
                    mailContent.UserEmail = singleMailAddr;

                    // 延遲 1 秒
                    Thread.Sleep(1000);

                    // 寄出 Mail
                    Models.ActionResponse<Models.MailResult> mailSellerNewOrder_MailResult = new Models.ActionResponse<Models.MailResult>();
                    API.Models.Connector connector = new Models.Connector();
                    mailSellerNewOrder_MailResult = connector.SendMail(null, null, mailContent);

                    // 判斷信件是否寄送成功，若失敗則再另外寄信通知管理人
                    if (mailSellerNewOrder_MailResult.IsSuccess)
                    {
                        this.sendMailResult.Add(logTitle + _mailAddrIndex.ToString(), string.Format("{0} 寄送成功。", singleMailAddr));
                    }
                    else
                    {
                        this.sendMailResult.Add(logTitle + _mailAddrIndex.ToString(), string.Format("{0} 寄送失敗。{1}", singleMailAddr, mailSellerNewOrder_MailResult.Msg));

                        sendAdminMail(orderNumber, productProcessInfo.ProductID, singleMailAddr, mailSellerNewOrder_MailResult.Msg + string.Format("({0})", logTitle));
                    }

                    log.Info("訂單信: " + orderNumber + ", 收件人: " + singleMailAddr + ", 寄送: " + mailSellerNewOrder_MailResult.IsSuccess);

                    _mailAddrIndex++;
                }

                #endregion

                processResult.IsSuccess = true;
                processResult.Msg = string.Empty;
                processResult.Code = (int)ResponseCode.Success;
                processResult.Body = sendMailResult;
            }
            catch (Exception ex)
            {
                processResult.IsSuccess = false;
                processResult.Msg = ex.Message + "(例外錯誤)";
                processResult.Code = (int)ResponseCode.Error;
                processResult.Body = sendMailResult;

                sendAdminMail(orderNumber, productProcessInfo.ProductID, "發生例外錯誤", ex.Message);

                log.Error("Order: " + orderNumber + ex.Message);
            }

            return processResult;
        }

        /// <summary>
        /// 判斷庫存量是否小於警示庫存量
        /// </summary>
        /// <param name="orderProductID">要查詢庫存的 ProductID </param>
        /// <returns>回傳是否低於安全庫存量</returns>
        private JudgeQty IsUnderStockSafeQty(int? orderProductID)
        {
            JudgeQty isUnderSafeQty;

            DB.TWSELLERPORTALDB.Models.Seller_ProductDetail detailInfo = new DB.TWSELLERPORTALDB.Models.Seller_ProductDetail();
            DB.TWSQLDB.Models.ItemStock forentitemStock = new ItemStock();

            forentitemStock = this.frontendSqldb.ItemStock.Where(x => x.ProductID == orderProductID).FirstOrDefault();
            detailInfo = this.sellerPortaldb.Seller_ProductDetail.Where(s => s.ProductID == orderProductID).FirstOrDefault();

            if (forentitemStock.SafeQty == 0)
            {
                isUnderSafeQty = JudgeQty.Unset;
            }
            else
            {
                //將前台 ItemStock Qty 回存到 Sellerportal productdetail Qty
                isUnderSafeQty = (forentitemStock.Qty - forentitemStock.QtyReg) <= forentitemStock.SafeQty ? JudgeQty.Under : JudgeQty.Nonunder;               
            }

            return isUnderSafeQty;
        }

        /// <summary>
        /// 判斷是否需要寄送庫存警示信
        /// </summary>
        /// <param name="inventoryMailtype">判斷是否有小於安全庫存量</param>
        /// <param name="productName">產品名稱，Mail 內提醒Seller的賣場名稱或產品名稱</param>
        /// <param name="userInfo">Sellerportal Seller_BasicInfo UserInfo</param>
        private bool IsSendInventoryMail(JudgeQty inventoryMailtype, string productName, DB.TWSELLERPORTALDB.Models.Seller_BasicInfo userInfo,List<string> userEmailList)
        {
            bool isSendMail = false;

            switch (inventoryMailtype)
            {
                case JudgeQty.Under:

                    ItemInventoryAlertService inventoryAlertService = new ItemInventoryAlertService();
                    ItemInventoryMailInfo inventoryMailInfo = new ItemInventoryMailInfo();
                    
                    // Find PMs Mail
                    var pms_Mail = this.Get_SellerEmailbyGroupID(5);

                    List<string> inventoryAlertRecipient = new List<string>();
                    
                    // 先加入基本 SellerMail
                    //inventoryAlertRecipient.Add(userInfo.SellerEmail);

                    inventoryAlertRecipient.AddRange(userEmailList);
                    
                    inventoryAlertRecipient.AddRange(pms_Mail);

                    // 去除重複收件人
                    inventoryAlertRecipient = inventoryAlertRecipient.Distinct().ToList();

                    // 開始寄送庫存警示信
                    foreach (var sendMailIndex in inventoryAlertRecipient)
                    {
                        inventoryMailInfo.ProductName = productName;
                        inventoryMailInfo.UserEmail = sendMailIndex;
                        inventoryMailInfo.UserName = string.IsNullOrEmpty(userInfo.SellerName) ? userInfo.SellerEmail : userInfo.SellerName;
                        var sendResult = inventoryAlertService.SendInventoryAlertEmail(inventoryMailInfo);
                        isSendMail = sendResult.IsSuccess;
                        if (sendResult.IsSuccess)
                        {
                            sendMailResult.Add("庫存警示信" + inventoryAlertRecipient.IndexOf(sendMailIndex), string.Format("商品庫存量小於等於庫存警示量，寄送成功。mail:{0}", inventoryMailInfo.UserEmail));
                        }
                        else
                        {
                            sendMailResult.Add("庫存警示信" + inventoryAlertRecipient.IndexOf(sendMailIndex), sendResult.Msg + "，寄送失敗。mail: " + inventoryMailInfo.UserEmail);

                            sendAdminMail("庫存", 0, userInfo.SellerEmail, sendResult.Msg + "(庫存警示信)");
                        }

                        log.Info("庫存信, 收件人: " + sendMailIndex + ", 寄送: " + sendResult.IsSuccess);
                        
                        Thread.Sleep(1000);
                    }

                    break;
                case JudgeQty.Unset:
                    sendMailResult.Add("庫存警示信", "商品庫存量未設定庫存警示量，未寄送。");
                    break;
                case JudgeQty.Nonunder:
                    sendMailResult.Add("庫存警示信", "商品庫存量未小於庫存警示量，未寄送。");
                    break;
            }

            return isSendMail;
        }

        #region 填寫通知信

        /// <summary>
        /// 填寫通知信
        /// </summary>
        /// <param name="notificationType">通知信類型</param>
        /// <param name="userInfo">商家資訊</param>
        /// <param name="orderNumber">訂單編號</param>
        /// <returns>通知信內容</returns>
        private API.Models.Mail FillMail(NotificationTypeCode notificationType, DB.TWSELLERPORTALDB.Models.Seller_BasicInfo userInfo, string orderNumber)
        {
            API.Models.Mail mail = new API.Models.Mail();

            // 商家名稱
            mail.UserName = string.IsNullOrEmpty(userInfo.SellerName) ? userInfo.EmailAddress : userInfo.SellerName; //20140626 sellerName 有可能是空白 則填入email address

            switch (notificationType)
            {
                case NotificationTypeCode.ON:
                    {
                        // 填寫新訂單通知信
                        mail = FillOrderNotificationsMail(mail, orderNumber);
                        break;
                    }
                case NotificationTypeCode.VON:
                    {
                        // 填寫訂單取消通知信
                        mail = FillVoidOrderNotificationsMail(mail, orderNumber);
                        break;
                    }
                default:
                    {
                        mail = null;
                        break;
                    }
            }

            return mail;
        }

        /// <summary>
        /// 填寫新訂單通知信
        /// </summary>
        /// <param name="orderNumber">訂單編號</param>
        /// <returns>通知信內容</returns>
        private API.Models.Mail FillOrderNotificationsMail(API.Models.Mail mail, string orderNumber)
        {
            // 指定信件格式為新訂單通知信
            mail.MailType = Models.Mail.MailTypeEnum.InformSellerNewSalesOrder;

            // 讀取訂單建立日期
            DateTime? orderDate = (from a in backendSqldb.Cart where a.ID == orderNumber select new { a.CreateDate }).ToList().FirstOrDefault().CreateDate;

            // 若查無訂單建立日期，則給一個初始值
            if (orderDate == null)
            {
                // DateTime.MinValue = 0001 年元月 1 日 00:00:00.0000000
                orderDate = DateTime.MinValue;
            }

            // 信件內容
            mail.MailMessage = string.Format("{0},{1:yyyy/MM/dd hh:mm}", orderNumber, orderDate);

            return mail;
        }

        /// <summary>
        /// 填寫訂單取消通知信
        /// </summary>
        /// <param name="orderNumber">訂單編號</param>
        /// <returns>通知信內容</returns>
        private API.Models.Mail FillVoidOrderNotificationsMail(API.Models.Mail mail, string orderNumber)
        {
            // 指定信件格式為訂單取消通知信
            mail.MailType = Models.Mail.MailTypeEnum.InformSellerCancelOrder;

            // 訂單號碼
            mail.MailMessage = orderNumber;

            return mail;
        }

        #endregion 填寫通知信

        #endregion 訂單處理，寄送訂單通知、庫存警示信

        /// <summary>
        /// 訂單取消通知
        /// </summary>
        /// <param name="orderNumber">訂單編號</param>
        /// <returns>成功及失敗訊息</returns>
        public ActionResponse<Dictionary<string, string>> ProcessVoidOrderNumMail(string orderNumber)
        {
            ActionResponse<Dictionary<string, string>> processResult = new ActionResponse<Dictionary<string, string>>();

            // 讀取環境設定
            API.Models.Connector connector = new Connector();
            string website = connector.GetAPIWebConfigSetting("TWSPHost");

            // 記錄訊息
            log.Info("訂單取消通知 CartID: " + orderNumber);
            temporderNum = orderNumber;

            try
            {
                #region 查詢購物車子單

                // 依訂單編號，查詢後台購物車子單
                DB.TWBACKENDDB.Models.Process productProcessInfo = this.backendSqldb.Process.Where(x => x.CartID == orderNumber).FirstOrDefault();
                
                #endregion 查詢購物車子單
                
                #region 查詢前台商品資訊

                // 前台商品資訊
                DB.TWSQLDB.Models.Product productInfo = new DB.TWSQLDB.Models.Product();

                // 若查到購物車子單，才進行商品資訊查詢
                if (productProcessInfo != null)
                {
                    // 依商品編號，查詢前台商品資訊
                    productInfo = this.frontendSqldb.Product.Where(x => x.ID == productProcessInfo.ProductID).FirstOrDefault();
                }
                else
                {
                    // 查無購物車子單，商品資訊直接設為 null
                    productInfo = null;
                }

                #endregion 查詢前台商品資訊

                #region 查詢商家資訊

                // 商家資訊
                DB.TWSELLERPORTALDB.Models.Seller_BasicInfo userInfo = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();

                // 若查到商品資訊，才進行商家資訊查詢
                if (productInfo != null)
                {
                    // 依商家編號，查詢商家資訊
                    userInfo = this.sellerPortaldb.Seller_BasicInfo.Where(x => x.SellerID == productInfo.SellerID).FirstOrDefault();
                }
                else
                {
                    // 查無商品資訊，商家資訊直接設為 null
                    userInfo = null;
                }

                #endregion 查詢商家資訊

                #region 查詢通知對象

                // 通知對象列表
                string[] userEmailStr_Array;

                // 若查到商家資訊，才進行通知對象查詢
                if (userInfo != null)
                {
                    // 查詢商家設定的通知對象
                    userEmailStr_Array = SearchSellerEmail(userInfo.SellerID.ToString(), NotificationTypeCode.VON);
                }
                else
                {
                    // 查無商家資訊，通知對象列表直接設為 null
                    userEmailStr_Array = null;
                }

                #endregion 查詢通知對象

                #region 寄出訂單取消通知信

                if (productProcessInfo != null
                && productInfo != null
                && userInfo != null
                && (userEmailStr_Array != null && userEmailStr_Array.Count() > 0))
                {
                    switch (website)
                    {
                        case "http://sellerportal.newegg.com.tw":
                            {
                                processResult = SendProcessOrderMail(orderNumber, productProcessInfo, userInfo, userEmailStr_Array, NotificationTypeCode.VON);
                                break;
                            }
                        default:
                            {
                                processResult = TestProcessOrderMail(userEmailStr_Array, userInfo.SellerEmail, NotificationTypeCode.VON);
                                break;
                            }
                    }
                }
                else
                {
                    // 錯誤訊息
                    if (productProcessInfo == null)
                    {
                        processResult.Msg += " 查無購物車子單資料。";
                    }

                    if (productInfo == null)
                    {
                        processResult.Msg += " 查無商品資訊資料。";
                    }

                    if (userInfo == null)
                    {
                        processResult.Msg += " 查無商家資訊資料。";
                    }

                    if (userEmailStr_Array == null || userEmailStr_Array.Count() <= 0)
                    {
                        processResult.Msg += " 查無通知對象。";
                    }

                    processResult.IsSuccess = false;
                    processResult.Code = (int)ResponseCode.Error;
                    processResult.Body = null;

                    // 記錄訊息
                    log.Error("orderNum: " + orderNumber + processResult.Msg);
                }

                #endregion 寄出訂單取消通知信
            }
            catch (Exception ex)
            {
                processResult.Msg = ex.ToString();
                processResult.IsSuccess = false;
                processResult.Code = (int)ResponseCode.Error;
                processResult.Body = null;
            }

            return processResult;
        }

        #region 退貨單寄信處理

        /// <summary>
        /// 退貨通知信
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        public ActionResponse<string> ProcessRMAOrderNumMail(string orderNumber)
        {
            // 讀取環境設定
            API.Models.Connector connector = new Connector();

            ActionResponse<string> result = new ActionResponse<string>();

            var vendorCart = backendSqldb.Cart.Where(x => x.ID == orderNumber).FirstOrDefault();

            var VendorProcess = backendSqldb.Process.Where(x => x.CartID == orderNumber).FirstOrDefault();

            var VendorID = frontendSqldb.Item.Where(x => x.ID == VendorProcess.StoreID).Select(r => r.SellerID).FirstOrDefault();

            var userInfo = sellerPortaldb.Seller_BasicInfo.Where(x => x.SellerID == VendorID).FirstOrDefault();

            // 放入寄給管理者的訂單號
            temporderNum = orderNumber;

            // 若 Cart 有資料
            if (vendorCart != null && userInfo != null)
            {
                #region 查詢通知對象

                // 通知對象列表
                string[] userEmailStr_Array;

                // 若查到商家資訊，才進行通知對象查詢
                if (userInfo != null)
                {
                    // 查詢商家設定的通知對象
                    userEmailStr_Array = SearchSellerEmail(userInfo.SellerID.ToString(), NotificationTypeCode.RMA);
                }
                else
                {
                    // 查無商家資訊，通知對象列表直接設為 null
                    userEmailStr_Array = null;
                }

                #endregion 查詢通知對象

                DB.TWSQLDB.Models.SalesOrder.DelivTypeList cartDelivType = (DB.TWSQLDB.Models.SalesOrder.DelivTypeList)vendorCart.ShipType;

                switch (cartDelivType)
                {
                    // 只處理非寄倉的
                    case SalesOrder.DelivTypeList.DirectShipment:
                    case SalesOrder.DelivTypeList.B2CDirectShipment:
                        {
                            // 寄給 Vendor or Seller
                            result.Body = this.SendRMAOrderMail(userEmailStr_Array, orderNumber, userInfo);

                            if (result.Body == string.Empty)
                            {
                                result.IsSuccess = true;
                                result.Code = (int)ResponseCode.Success;
                                result.Msg = "退貨單寄送成功";

                                log.Info("退貨單寄送成功，訂單編號: " + orderNumber);
                            }
                            else
                            {
                                result.IsSuccess = false;
                                result.Code = (int)ResponseCode.Error;
                                result.Msg = "退貨單寄送失敗";

                                log.Error("退貨單寄送失敗: " + result.Body);
                            }
                        }
                        break;
                    default:
                        {
                            //通知其他交易模式的退貨單
                            //this.SendWarehouseMail(userEmailStr_Array, orderNumber, userInfo);
                        }
                        break;

                }
            }
            else
            {
                this.sendAdminMail("退貨", null, string.Empty, "此訂單查無 Cart 及 供應商資料");
                log.Error("退貨單寄送失敗: " + orderNumber + " 查無 Cart 及 供應商資料");
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = "此訂單查無 Cart 及 供應商資料";
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userEmailStr_Array"></param>
        /// <param name="orderNumber"></param>
        /// <param name="UserInfo"></param>
        private void SendWarehouseMail(string[] userEmailStr_Array, string orderNumber, TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo UserInfo)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userEmailStr_Array"></param>
        /// <param name="orderNumber"></param>
        /// <param name="UserInfo"></param>
        private string SendRMAOrderMail(string[] userEmailStr_Array, string orderNumber, TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo UserInfo)
        {
            string result = string.Empty;

            TWNewEgg.API.Models.Connector conn = new Connector();

            TWNewEgg.API.Models.Mail mail = new Mail();
            mail.MailType = Mail.MailTypeEnum.RMAInfo;
            mail.UserName = UserInfo.SellerName;
            // 信件內容
            mail.MailMessage = orderNumber;
            mail.OrderInfo.OrderID = orderNumber;
            mail.OrderInfo.ProductName = this.backendSqldb.Process.Where(x => x.CartID == orderNumber).Select(y => y.Title).FirstOrDefault();
            
            foreach (var userMail in userEmailStr_Array)
            {
                mail.UserEmail = userMail;

                var sendResult = conn.SendMail(null, null, mail);

                if (sendResult.IsSuccess == false)
                {
                    result = string.IsNullOrEmpty(result) ? mail.UserEmail + "寄送失敗" : result + mail.UserEmail + "寄送失敗, ";
                }

                Thread.Sleep(1000);
            }

            return result;
        }


        #endregion

        #region 退貨成功通知信

        public ActionResponse<string> ProcessRMASuccess(string orderNumber)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.API.Models.Connector conn = new Connector();

            result.Body = string.Empty;

            var vendorCart = backendSqldb.Cart.Where(x => x.ID == orderNumber).FirstOrDefault();

            var VendorProcess = backendSqldb.Process.Where(x => x.CartID == orderNumber).FirstOrDefault();

            var VendorID = frontendSqldb.Item.Where(x => x.ID == VendorProcess.StoreID).Select(r => r.SellerID).FirstOrDefault();

            var userInfo = sellerPortaldb.Seller_BasicInfo.Where(x => x.SellerID == VendorID).FirstOrDefault();

            string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            Models.Mail mailContent = new Models.Mail();

            string[] errorInfo = adminEmail.Split(',');

            TWNewEgg.API.Models.Mail mail = new Mail();
            mail.MailType = Mail.MailTypeEnum.RMASuccessInfo;
            mail.UserName = "供應商管理人員";
            // 信件內容
            mail.MailMessage = userInfo.SellerName + " 訂單: " + orderNumber + ", 已經完成退貨處理。";

            foreach (var userMail in errorInfo)
            {
                mail.UserEmail = userMail;

                var sendResult = conn.SendMail(null, null, mail);

                if (sendResult.IsSuccess == false)
                {
                    result.Body = string.IsNullOrEmpty(result.Body) ? mail.UserEmail + "寄送失敗" : result.Body + mail.UserEmail + "寄送失敗, ";
                }

                Thread.Sleep(1000);
            }

            if (string.IsNullOrEmpty(result.Body))
            {
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = "退貨完成通知信寄送成功。";
            }
            else
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = "退貨完成通知信寄送失敗。";
            }

            return result;
        }

        #endregion


        /// <summary>
        /// 取得不重複Email列表
        /// </summary>
        /// <value>Email 不重複列表</value>
        public IEnumerable<string> List_EmailAddr_Dictinct { get; set; }

        /// <summary>
        /// 取得特定 GroupID 的 Emails
        /// </summary>
        /// <param name="groupID">groupID</param>
        /// <returns>回傳一個Email List</returns>
        private IEnumerable<string> Get_SellerEmailbyGroupID(int groupID)
        {
            return this.sellerPortaldb.Seller_User.Where(x => x.GroupID == groupID).Select(e => e.UserEmail).ToList();
        }
    }
}
