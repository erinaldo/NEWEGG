using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.OeyaIChannelsService.Service
{
    public class OeyaIChannelsService
    {
        string companyCode = "%2FHCB1%2FSs2GzA8IZp";
        string oeyaIChannelsOrderInfoReceiveUrl = "http://www.ichannels.com.tw/manu_order_code.php";

        public void SendToOeya(string soiCode)
        {
            DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
            var orderInfo = backendDB.OeyaIChannelsOrderInfo.Where(x => x.SalesOrderItemCode == soiCode).FirstOrDefault();

            if (orderInfo != null)
            {
                //convert data
                var infoOutList = ConvertToOrderInfoOut(orderInfo);

                string response = null;
                infoOutList.ForEach(infoOut =>
                {
                    //發送訂單資料，最多嘗試連線五次
                    for (int retry = 0; retry < 5; retry++)
                    {
                        try
                        {
                            string resCode = SendOrderInfoToOeya(infoOut);
                            if (resCode == "")
                            {
                                response += "_Y";
                            }
                            else
                            {
                                response += "_" + resCode;
                            }
                            //連線成功，跳出迴圈
                            break;
                        }
                        catch (Exception ex)
                        {
                            response += "_" + ex.Message;
                        }
                    }
                });

                if (response == null)
                {
                    response = "";
                }

                if (response.Length > 1500)
                {
                    response = response.Substring(0, 1500);
                }

                orderInfo.SendStatus = response;
                orderInfo.SendDate = DateTime.UtcNow.AddHours(8);
                backendDB.SaveChanges();
            }
        }

        /// <summary>
        /// 傳送訂單資料給iChannels通路王
        /// </summary>
        /// <param name="orderInfo">欲發送的訂單資料</param>
        /// <returns>空字串=發送成功，1=訂單編號重複，2=推薦代碼錯誤，3=廠商編碼錯誤</returns>
        public string SendOrderInfoToOeya(Models.OrderInfoOut orderInfoOut)
        {
            string result = null;

            //組合 URL + 訂單資料
            System.Text.StringBuilder urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(this.oeyaIChannelsOrderInfoReceiveUrl);
            urlBuilder.AppendFormat("?a_id={0}", orderInfoOut.OeyaInfo);
            urlBuilder.AppendFormat("&manu_code={0}", this.companyCode);
            urlBuilder.AppendFormat("&order_code={0}", orderInfoOut.SalesOrderItemCode);
            urlBuilder.AppendFormat("&pro_code={0}", orderInfoOut.ProductID);
            urlBuilder.AppendFormat("&pro_name={0}", orderInfoOut.ProductName);
            urlBuilder.AppendFormat("&price={0}", orderInfoOut.ProductPrice);
            urlBuilder.AppendFormat("&pro_cnt={0}", orderInfoOut.ProductQty);
            urlBuilder.AppendFormat("&count={0}", orderInfoOut.TotalPrice);
            urlBuilder.AppendFormat("&back_code={0}", orderInfoOut.BackCode);
            urlBuilder.AppendFormat("&other={0}", orderInfoOut.Other);

            //傳送資料 Http Method : Get
            System.Net.WebClient client = new System.Net.WebClient();
            string url = urlBuilder.ToString();
            result = client.DownloadString(url);

            return result;
        }

        /// <summary>
        /// 提供訂單資料給iChannels通路王查詢
        /// </summary>
        /// <returns></returns>
        public string QueryOrderInfo(DateTime start_time, DateTime end_time)
        {
            //iChannels通路王會傳入 start_time=2014-07-01 end_time=2014-07-10
            //因為沒有指定"時分秒"，會造成時分秒都是零的狀況
            //為避免查詢結果未涵蓋所需的資料範圍，故將end_tiem增加一天
            end_time = end_time.AddDays(1);

            DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
            var orderInfoList = backendDB.OeyaIChannelsOrderInfo.Where(x => x.OrderCreateDate >= start_time && x.OrderCreateDate <= end_time).ToList();
            StringBuilder responseDataBuilder = new StringBuilder();
            orderInfoList.ForEach(orderInfo =>
            {
                orderInfo.OrderStatus = CheckOrderStatus(orderInfo.SalesOrderItemCode);
                orderInfo.OrderStatusDate = DateTime.UtcNow.AddHours(8);
                orderInfo.OeyaLastQueryDate = DateTime.UtcNow.AddHours(8);

                //convert data
                var infoOutList = ConvertToOrderInfoOut(orderInfo);

                infoOutList.ForEach(infoOut =>
                {
                    responseDataBuilder.Append(infoOut.OrderCreateDate);
                    responseDataBuilder.Append("," + infoOut.OeyaInfo);
                    responseDataBuilder.Append(",");
                    responseDataBuilder.Append("," + infoOut.OrderStatus);
                    responseDataBuilder.Append(",");
                    responseDataBuilder.Append("," + infoOut.SalesOrderItemCode);
                    responseDataBuilder.Append("," + infoOut.ProductID);
                    responseDataBuilder.Append("," + infoOut.ProductName);
                    responseDataBuilder.Append("," + infoOut.ProductPrice);
                    responseDataBuilder.Append("," + infoOut.ProductQty);
                    responseDataBuilder.Append("," + infoOut.BackCode);
                    responseDataBuilder.Append("," + infoOut.TotalPrice);
                    responseDataBuilder.Append(",");
                    responseDataBuilder.Append(",");
                    responseDataBuilder.Append("," + infoOut.InvalidReason);
                    responseDataBuilder.Append("\n");
                });
            });
            backendDB.SaveChanges();

            return responseDataBuilder.ToString();
        }

        public List<Models.OrderInfoOut> ConvertToOrderInfoOut(DB.TWBACKENDDB.Models.OeyaIChannelsOrderInfo order)
        {
            List<Models.OrderInfoOut> result = new List<Models.OrderInfoOut>();

            string[] backCodeList = order.BackCode.Replace("||", ",").Split(',');
            string productID = "";
            string productName = "";
            string productPrice = "";
            string productQty = "";
            decimal totalPrice = 0;
            string backCode = "";

            for (int i = 0; i < backCodeList.Length; i++)
            {
                string code = backCodeList[i];
                if (code.IndexOf("[") >= 0)
                {
                    //CPL、CPA 合作項目，必須獨立成為一張訂單拋送
                    //訂單編號格式 : 原始訂單子單編號 + "_" + 合作項目編號
                    Models.OrderInfoOut specialOrder = new Models.OrderInfoOut();
                    specialOrder.OeyaInfo = System.Web.HttpUtility.UrlEncode(order.OeyaInfo);
                    specialOrder.SalesOrderItemCode = order.SalesOrderItemCode + "_" + code;
                    specialOrder.ProductID = order.ProductID;
                    specialOrder.ProductName = order.ProductName.Replace(",","_");
                    specialOrder.ProductPrice = "0";
                    specialOrder.ProductQty = "1";
                    specialOrder.TotalPrice = "0";
                    specialOrder.OrderStatus = order.OrderStatus.ToString();
                    specialOrder.BackCode = code.Replace("[", "").Replace("]", "");
                    specialOrder.InvalidReason = order.InvalidReason.Replace(",", "_");
                    specialOrder.Other = order.Other.Replace(",", "_");
                    specialOrder.OrderCreateDate = order.OrderCreateDate.ToString("yyyy-MM-dd HH:mm:ss");

                    result.Add(specialOrder);
                }
                else
                {
                    productPrice += "||" + order.ProductPrice.ToString();
                    productQty += "||" + order.ProductQty.ToString();
                    totalPrice += order.ProductPrice * (decimal)order.ProductQty;
                    productID += "||" + order.ProductID;
                    productName += "||" + order.ProductName.Replace(",", "_");
                    backCode += "||" + code;
                }

            }

            //截掉開頭多餘的分隔符號 "||"
            if (productID.Length > 2)
            {
                productID = productID.Substring(2);
            }
            if (productName.Length > 2)
            {
                productName = productName.Substring(2);
            }
            if (productPrice.Length > 2)
            {
                productPrice = productPrice.Substring(2);
            }
            if (productQty.Length > 2)
            {
                productQty = productQty.Substring(2);
            }
            if (backCode.Length > 2)
            {
                backCode = backCode.Substring(2);
            }

            Models.OrderInfoOut orderInfoOut = new Models.OrderInfoOut();
            orderInfoOut.OeyaInfo = System.Web.HttpUtility.UrlEncode(order.OeyaInfo);
            orderInfoOut.SalesOrderItemCode = order.SalesOrderItemCode;
            orderInfoOut.ProductID = productID;
            orderInfoOut.ProductName = productName;
            orderInfoOut.ProductPrice = productPrice;
            orderInfoOut.ProductQty = productQty;
            orderInfoOut.TotalPrice = totalPrice.ToString();
            orderInfoOut.OrderStatus = order.OrderStatus.ToString();
            orderInfoOut.BackCode = backCode;
            orderInfoOut.InvalidReason = order.InvalidReason;
            orderInfoOut.Other = order.Other;
            orderInfoOut.OrderCreateDate = order.OrderCreateDate.ToString("yyyy-MM-dd HH:mm:ss");

            result.Add(orderInfoOut);

            return result;
        }

        /// <summary>
        /// 儲存訂單資料提供給iChannels通路王
        /// </summary>
        /// <param name="salesOrderCode">訂單編號</param>
        /// <param name="oeyaCookie">COOKIE[OEYA]推薦資訊</param>
        public void SaveOrderInfoToOeyaIChannelsOrderInfo(string salesOrderCode, string oeyaCookie)
        {
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();

            var so = db.SalesOrder.Where(x => x.Code == salesOrderCode).FirstOrDefault();
            if (so != null)
            {
                var soiList = db.SalesOrderItem.Where(x => x.SalesorderCode == so.Code).ToList();
                soiList.ForEach(process =>
                {
                    //訂單子單Process 轉換為iChannels通路王訂單資訊OeyaIChannelsOrderInfo
                    var oeyaOrderInfo = ConvertSalesOrderItemToOeyaIChannelsOrderInfo(process);
                    oeyaOrderInfo.OeyaInfo = oeyaCookie;
                    DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
                    if (backendDB.OeyaIChannelsOrderInfo.Where(x => x.SalesOrderItemCode == oeyaOrderInfo.SalesOrderItemCode).Count() <= 0)
                    {
                        backendDB.OeyaIChannelsOrderInfo.Add(oeyaOrderInfo);
                        try
                        {
                            backendDB.SaveChanges();
                        }
                        catch { }
                    }
                });
            }
        }

        /// <summary>
        /// 將訂單子單Process轉換為DB.TWBACKENDDB.Models.OeyaIChannelsOrderInfo
        /// </summary>
        /// <param name="soi">欲轉換的訂單子單Process</param>
        /// <returns></returns>
        public DB.TWBACKENDDB.Models.OeyaIChannelsOrderInfo ConvertSalesOrderItemToOeyaIChannelsOrderInfo(DB.TWSQLDB.Models.SalesOrderItem soi)
        {
            DB.TWBACKENDDB.Models.OeyaIChannelsOrderInfo result = new DB.TWBACKENDDB.Models.OeyaIChannelsOrderInfo();

            result.SalesOrderItemCode = soi.Code;
            result.ProductID = soi.ProductID.ToString();
            result.ProductName = soi.Name;
            result.ProductPrice = 0;
            //售價
            result.ProductPrice+=soi.Price;
            //金流服務費
            result.ProductPrice += soi.InstallmentFee;
            if (soi.ShippingExpense.HasValue)
            {
                //運費
                result.ProductPrice += soi.ShippingExpense.Value;
            }
            if (soi.ServiceExpense.HasValue)
            {
                //服務費
                result.ProductPrice += soi.ServiceExpense.Value;
            }
            if (soi.Pricecoupon.HasValue)
            {
                //折抵金額
                result.ProductPrice -= soi.Pricecoupon.Value;
            }
            result.ProductQty = soi.Qty;
            result.TotalPrice = result.ProductPrice * result.ProductQty;
            result.OrderStatus = 0;
            result.OrderStatusDate = DateTime.UtcNow.AddHours(8);
            result.OrderCreateDate = soi.CreateDate;
            result.OeyaInfo = "";
            result.BackCode = GenerateBackCode(soi.Code);
            result.InvalidReason = "";
            result.Other = "";
            result.SendStatus = "N";
            result.SendDate = null;
            result.OeyaLastQueryDate = null;

            return result;
        }

        public int CheckOrderStatus(string soiCode)
        {
            int status = 0; //0=未確認，1=有效，2=無效

            //從後台尋找這筆訂單的狀態
            DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
            var process = backendDB.Process.Where(x => x.ID == soiCode).FirstOrDefault();
            if (process != null)
            {
                var cart = backendDB.Cart.Where(x => x.ID == process.CartID).FirstOrDefault();
                if (cart != null)
                {
                    if (cart.Status == (int)DB.TWBACKENDDB.Models.Cart.status.正常 || cart.Status == (int)DB.TWBACKENDDB.Models.Cart.status.完成)
                    {
                        if (DateTime.UtcNow.AddHours(8) > cart.CreateDate.Value.AddDays(7))
                        {
                            //訂單狀態為[正常]、[完成]，且已超過鑑賞期，判定為有效訂單。
                            status = 1;
                        }
                        else
                        {
                            //訂單狀態為[正常]、[完成]，還未超過鑑賞期，判定為未確認。
                            status = 0;
                        }
                    }
                    else
                    {
                        //訂單狀態異常，判定為無效
                        status = 2;
                    }
                }
                else
                {
                    //找不到Cart資料，屬於異常狀況，判定為無效
                    status = 2;
                }
            }
            else
            {
                //後台資料庫找不到這筆訂單
                DB.TWSqlDBContext db = new DB.TWSqlDBContext();
                var soi = db.SalesOrderItem.Where(x => x.Code == soiCode).FirstOrDefault();
                if (soi != null)
                {
                    var so = db.SalesOrder.Where(x => x.Code == soi.SalesorderCode).FirstOrDefault();
                    if (so != null)
                    {
                        if (so.Status == (int)DB.TWSQLDB.Models.SalesOrder.status.付款成功 || so.Status == (int)DB.TWSQLDB.Models.SalesOrder.status.完成)
                        {
                            //訂單狀態正常，未轉單後台判定為未確認
                            status = 0;
                        }
                        else
                        {
                            //訂單狀態異常，判定為無效
                            status = 2;
                        }
                    }
                    else
                    {
                        //找不到SalesOrder資料，屬於異常狀況，判定為無效
                        status = 2;
                    }
                }
                else
                {
                    //前後台資料庫都找不到這筆訂單，判定為無效
                    status = 2;
                }
            }

            return status;
        }

        /// <summary>
        /// 檢查訂單資料，回傳符合的合作項目編號
        /// </summary>
        /// <param name="processID">訂單子單Process編號</param>
        /// <returns></returns>
        public string GenerateBackCode(string processID)
        {
            List<string> backCodeList = new List<string>();
            string result = "";

            //收集符合的項目編號
            //若為CPL(Cost-Per-Lead)、CPA(Cost Per Action)則項目編號左右加上中括號[123_1]
            string temp = "";
            //CommossionRuleOne 1316_1 全館商品(電腦/3C系列商品除外)	
            temp = CommossionRuleOne(processID);
            if (temp != "")
            {
                backCodeList.Add(temp);
            }

            //CommossionRuleOne [1316_2] 新會員入會有購 CPL
            temp = CommossionRuleTwo(processID);
            if (temp != "")
            {
                backCodeList.Add(temp);
            }

            //CommossionRuleOne 1316_3 電腦/3C類商品
            temp = CommossionRuleThree(processID);
            if (temp != "")
            {
                backCodeList.Add(temp);
            }

            //合併
            result = String.Join("||", backCodeList.ToArray());

            return result;
        }

        private string CommossionRuleOne(string soiCode)
        {
            //1316_1 全館商品(電腦/3C系列商品除外)
            //全館商品(電腦/3C系列商品除外)每筆成交訂單
            string code = "1316_1";
            int computer = 1; //電腦分類ID
            int threeC = 264; //3C分類ID
            string result = "";

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var soi = db.SalesOrderItem.Where(x => x.Code == soiCode).FirstOrDefault();
            if (soi != null)
            {
                var item = db.Item.Where(x => x.ID == soi.ItemID).FirstOrDefault();
                var categoryLayerThree = db.Category.Where(x => x.ID == item.CategoryID).FirstOrDefault();
                if (categoryLayerThree != null)
                {
                    var categoryLayerTwo = db.Category.Where(x => x.ID == categoryLayerThree.ParentID).FirstOrDefault();
                    if (categoryLayerTwo != null)
                    {
                        var categoryLayerOne = db.Category.Where(x => x.ID == categoryLayerTwo.ParentID).FirstOrDefault();
                        if (categoryLayerOne != null)
                        {
                            if (categoryLayerOne.ID != computer && categoryLayerOne.ID != threeC)
                            {
                                result = code;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private string CommossionRuleTwo(string soiCode)
        {
            //[1316_2] 新會員入會有購 CPL
            //新加入會員完成註冊並且在一個月內有完成購買即回饋
            //因為CPL(Cost-Per-Lead)，項目編號左右加上中括號[1316_2]
            string code = "[1316_2]";
            string result = "";

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var soi = db.SalesOrderItem.Where(x => x.Code == soiCode).FirstOrDefault();
            if (soi != null)
            {
                var so = db.SalesOrder.Where(x => x.Code == soi.SalesorderCode).FirstOrDefault();
                if (so != null)
                {
                    var account = db.Account.Where(x => x.ID == so.AccountID).FirstOrDefault();
                    //必須是第一筆SalesOrderItem才符合首購條件
                    var soCount = db.SalesOrder.Where(x => x.AccountID == so.AccountID && x.CreateDate < so.CreateDate).Count();
                    var firstSOICode = db.SalesOrderItem.Where(x => x.SalesorderCode == so.Code).Select(x => x.Code).First();
                    if (account != null && soCount == 0 && soiCode == firstSOICode)
                    {
                        var range = DateTime.UtcNow.AddHours(8) - account.CreateDate;
                        //必須是一個月內註冊的新會員
                        if (range.TotalDays <= 30)
                        {
                            result = code;
                        }
                    }
                }
            }

            return result;
        }

        private string CommossionRuleThree(string soiCode)
        {
            //1316_3 電腦/3C類商品
            //電腦/3C類商品每筆成交訂單
            string code = "1316_3";
            int computer = 1; //電腦分類ID
            int threeC = 264; //3C分類ID
            string result = "";

            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            var soi = db.SalesOrderItem.Where(x => x.Code == soiCode).FirstOrDefault();
            if (soi != null)
            {
                var item = db.Item.Where(x => x.ID == soi.ItemID).FirstOrDefault();
                var categoryLayerThree = db.Category.Where(x => x.ID == item.CategoryID).FirstOrDefault();
                if (categoryLayerThree != null)
                {
                    var categoryLayerTwo = db.Category.Where(x => x.ID == categoryLayerThree.ParentID).FirstOrDefault();
                    if (categoryLayerTwo != null)
                    {
                        var categoryLayerOne = db.Category.Where(x => x.ID == categoryLayerTwo.ParentID).FirstOrDefault();
                        if (categoryLayerOne != null)
                        {
                            if (categoryLayerOne.ID == computer || categoryLayerOne.ID == threeC)
                            {
                                result = code;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
