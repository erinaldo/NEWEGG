using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.ItemService.Models;
using TWNewEgg.Website.ECWeb.Models;

namespace TWNewEgg.Website.ECWeb.Controllers
{
    /// <summary>
    /// 分期付款
    /// </summary>
    public class InstallmentController : Controller
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        /// <summary>
        /// 組合出各分期加入利息後的金額與相關資訊
        /// </summary>
        /// <param name="buyingItemList">購物車資訊</param>
        /// <param name="cardList">付款模式清單</param>
        /// <param name="priceSum">購物車原始總金額</param>
        /// <param name="couponPrice">Coupon折價金額</param>
        /// <param name="discountAmount">滿額贈折價總金額</param>
        /// <returns>各分期加入利息後的金額與相關資訊</returns>
        public List<InstallmentInfo> InstallmentQuery(List<BuyingItems> buyingItemList, List<PayType> cardList, decimal priceSum, decimal couponPrice, decimal discountAmount)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            // 各分期加入利息後的金額與相關資訊的List
            List<InstallmentInfo> installmentInfoList = new List<InstallmentInfo>();
            // 取得購物車中所有商品的詳細資訊
            List<int> itemIDList = buyingItemList.Select(x => x.buyItemID).Distinct().ToList();
            List<Item> itemList = db_before.Item.Where(x => itemIDList.Contains(x.ID)).Distinct().ToList();
            // 取得所有有息分期且有啟動的Bank資訊, 目前為12期有息分期、18期有息分期、24期有息分期的銀行
            List<int> bankIDList = cardList.Where(x => (x.PayType0rateNum == (int)PayType.nPayType.十二期分期 || x.PayType0rateNum == (int)PayType.nPayType.十八期分期 || x.PayType0rateNum == (int)PayType.nPayType.二十四期分期) && x.Status == (int)PayType.PayTypeStatus.啟動).OrderBy(x => x.BankID).Select(x => (int)x.BankID).ToList();
            List<Bank> installmentBank = db_before.Bank.Where(x => bankIDList.Contains(x.ID)).Distinct().ToList();
            // 取出有息分期且啟動中的付款資訊, 目前為12期有息分期、18期有息分期、24期有息分期
            List<PayType> installmentPayType = cardList.Where(x => (x.PayType0rateNum == (int)PayType.nPayType.十二期分期 || x.PayType0rateNum == (int)PayType.nPayType.十八期分期 || x.PayType0rateNum == (int)PayType.nPayType.二十四期分期) && x.Status == (int)PayType.PayTypeStatus.啟動).OrderBy(x => x.PayType0rateNum).ToList();
            // 蒐集所有有息分期的詳細資訊
            foreach (Bank subBank in installmentBank)
            {
                // 取得PayType Table中該銀行有息分期且啟動中的所有付款類型
                List<PayType> subPayTypeList = installmentPayType.Where(x => x.BankID == subBank.ID).OrderBy(x => x.PayType0rateNum).ToList();
                // 將所有需要的資訊填入InstallmentInfo的Model中
                foreach (PayType subPayType in subPayTypeList)
                {
                    InstallmentInfo installmentInfo = new InstallmentInfo();
                    // 分期銀行ID
                    installmentInfo.BankID = subBank.ID;
                    // 分期銀行代碼
                    installmentInfo.BankCode = subBank.Code;
                    // 分期銀行名稱
                    installmentInfo.BankName = subBank.Name;
                    // 付款方式ID
                    installmentInfo.PayTypeID = subPayType.ID;
                    // 分期利率名稱
                    installmentInfo.InsRateName = subPayType.Name;
                    // 分期利率顯示名稱 ex. 12期 5%利率
                    installmentInfo.InsRateShowName = subPayType.Name + " " + (decimal.Round((decimal)subPayType.InsRate * 100, 1)).ToString() + "%利率";
                    // 分期期數設定，對應PayType Table 中的 PayType0rateNum
                    installmentInfo.PayType0rateNum = (int)subPayType.PayType0rateNum;
                    // 分期利率 先檢查商品是否為有息，再由付款方式中取得利息的利率值
                    installmentInfo.InsRate = (subPayType.PayType0rateType == (int)PayType.PayTypeRateType.有息) ? (subPayType.InsRate ?? 0m) : 0m;
                    // Coupon折價金額
                    installmentInfo.CouponPrice = couponPrice;
                    // 滿額贈折價總金額
                    installmentInfo.DiscountAmount = discountAmount;
                    // 未扣除折價金額且不含利息總金額
                    installmentInfo.OriginalPriceSum = priceSum;
                    // 利息(扣除折價金額後的金額) = 取得利息總額 GetInsRateFee(付款類型ID, 未扣除折價金額且不含利息總金額, 折價卷總金額, 滿額贈折價總金額)
                    installmentInfo.TotalInsRateFees = GetInsRateFee(subPayType.ID, priceSum, couponPrice, discountAmount);
                    // 加入扣除折價金額後再加入利息的總金額 = ((購物車總金額 - 折價卷總金額) * ( 1 + 分期利率)) 再將此金額執行四捨五入
                    installmentInfo.NewPriceSum = Math.Floor(0.5m + ((priceSum - couponPrice - discountAmount) * (1 + installmentInfo.InsRate)));
                    // 將資料填寫完後加入installmentInfoList中
                    installmentInfoList.Add(installmentInfo);
                }
            }
            // 返回各分期加入利息後的金額與相關資訊的List
            return installmentInfoList;
        }

        /// <summary>
        /// 取得利息總額
        /// </summary>
        /// <param name="payTypeID">付款類型ID</param>
        /// <param name="priceSum">未扣除折價卷與加入利息的總金額</param>
        /// <param name="couponPrice">Coupon折價卷總金額</param>
        /// <param name="discountAmount">滿額贈折價總金額</param>
        /// <returns>返回利息總額</returns>
        public decimal GetInsRateFee(int payTypeID, decimal priceSum, decimal couponPrice, decimal discountAmount)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            // 利息總額
            decimal insRateFee = 0m;
            // 取得的付款類型資訊
            PayType getPayType = null;
            // 分期利率
            decimal insRate = 0m;

            // 取得該付款類型詳細資訊
            getPayType = db_before.PayType.Where(x => x.ID == payTypeID).FirstOrDefault();
            // 先檢查商品是否為有息，再由付款方式中取得利息的利率值
            insRate = (getPayType.PayType0rateType == (int)PayType.PayTypeRateType.有息) ? (getPayType.InsRate ?? 0m) : 0m;
            // 當利率設定大於0時才執行
            if (insRate > 0)
            {
                // 取得利息總額 = ((未扣除折價卷與加入利息的總金額 - Coupon折價卷總金額 - 滿額贈折價總金額) * 分期利率) 再執行四捨五入
                insRateFee = Math.Floor(0.5m + ((priceSum - couponPrice - discountAmount) * insRate));
            }
            else
            {
                // 若該模式是有利息的，則必須設定利率，若無設定則表示該付款模式設定有誤
                if (getPayType.PayType0rateType == (int)PayType.PayTypeRateType.有息)
                {
                    logger.Info("PayTypeID[" + payTypeID + "] without InsRate");
                }
            }
            // 返回利息總額
            return insRateFee;
        }

        /// <summary>
        /// 1.將利息分配並儲存到SalesOrderItem.InstallmentFee欄位中 2.將收集到的資訊儲存並返回
        /// </summary>
        /// <param name="salesOrderGroupID">購物車編號</param>
        /// <returns>成功執行則返回該購物車與利息相關的資訊並設定SaveSuccess = true，反之則false</returns>
        public InstallmentInfo InstallmentInsert(int salesOrderGroupID)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            InstallmentInfo getInstallmentInfo = new InstallmentInfo();
            // 未扣除折價卷前的購物車總金額
            decimal cartPriceSum = 0m;
            // 折價卷總金額
            decimal totalCouponPrice = 0m;
            // 滿額贈折扣總金額
            decimal totalDiscountAmount = 0m;
            // 利息總額
            decimal insRateFees = 0m;
            // 利息暫存總額
            decimal insRateFeesTemp = 0m;
            // 搜尋出主單List並依訂單編號排序
            List<SalesOrder> salesOrderList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == salesOrderGroupID).OrderBy(x => x.Code).ToList();
            List<string> salesOrderCodeList = salesOrderList.Select(x => x.Code).Distinct().ToList();
            // 搜尋出子單List並依訂單編號排序
            List<SalesOrderItem> salesOrderItemList = db_before.SalesOrderItem.Where(x => salesOrderCodeList.Contains(x.SalesorderCode)).OrderBy(x => x.Code).ToList();
            // 找出此筆訂單中是使用哪種付款方式
            int payTypeID = (int)salesOrderList[0].PayTypeID;
            PayType payType = db_before.PayType.Where(x => x.ID == payTypeID).FirstOrDefault();
            // 先檢查商品是否為有息，再由付款方式中取得利息的利率值
            decimal insRate = (payType.PayType0rateType == (int)PayType.PayTypeRateType.有息) ? (payType.InsRate ?? 0m) : 0m;
            // 計算購物車總金額
            foreach (SalesOrder subSalesOrder in salesOrderList)
            {
                List<SalesOrderItem> salesOrderItems = salesOrderItemList.Where(x => x.SalesorderCode == subSalesOrder.Code).ToList();
                foreach (SalesOrderItem subSalesOrderItem in salesOrderItems)
                {
                    // 未扣除折價卷前的總金額 += SalesOrderItem.Price(商品金額+稅) + 部分運費 + 部分服務費
                    cartPriceSum += subSalesOrderItem.Price + (decimal)subSalesOrderItem.ShippingExpense + (decimal)subSalesOrderItem.ServiceExpense;
                    // 折價卷總金額
                    totalCouponPrice += (decimal)subSalesOrderItem.Pricecoupon;
                    // 滿額贈折扣總金額
                    totalDiscountAmount += subSalesOrderItem.ApportionedAmount;
                }
            }

            if (payType.PayType0rateType == (int)PayType.PayTypeRateType.有息)
            {
                // 利息總額 = ((未扣除折價卷前的總金額 - 折價卷總金額 - 滿額贈折扣總金額) * 利息利率) 再執行四捨五入
                insRateFees = Math.Floor(0.5m + ((cartPriceSum - totalCouponPrice - totalDiscountAmount) * insRate));
            }

            // 分配利息金額
            foreach (SalesOrder subSalesOrder in salesOrderList)
            {
                List<SalesOrderItem> salesOrderItems = salesOrderItemList.Where(x => x.SalesorderCode == subSalesOrder.Code).ToList();
                foreach (SalesOrderItem subSalesOrderItem in salesOrderItems)
                {
                    // 利息分攤 = ((SalesOrderItem.Price(商品金額+稅) + 分攤的部分運費 + 分攤的部分服務費 - Coupon折價金額 - 滿額贈折扣金額) * 利息利率) 再執行四捨五入
                    subSalesOrderItem.InstallmentFee = Math.Floor(0.5m + ((subSalesOrderItem.Price + (decimal)subSalesOrderItem.ShippingExpense + (decimal)subSalesOrderItem.ServiceExpense - (subSalesOrderItem.Pricecoupon ?? 0m) - subSalesOrderItem.ApportionedAmount) * insRate));
                    // 累計利息，多或少則補入該購物車中第一筆商品
                    insRateFeesTemp += subSalesOrderItem.InstallmentFee;
                }
            }
            // 若利息總額 != 利息暫存總額 則將不足或多於者補至該購物車中第一筆商品的利息金額中
            if (insRateFees != insRateFeesTemp)
            {
                // 將多或少的利息金額補入第一筆SalesOrderItem中 += 利息總額 - 利息暫存總額
                salesOrderItemList[0].InstallmentFee += insRateFees - insRateFeesTemp;
            }
            // 分期銀行ID
            getInstallmentInfo.BankID = (int)payType.BankID;
            // 分期銀行代碼
            getInstallmentInfo.BankCode = salesOrderList[0].CardBank;
            // 分期付款類型ID
            getInstallmentInfo.PayTypeID = payType.ID;
            // 分期付款類型名稱
            getInstallmentInfo.InsRateName = payType.Name;
            // 分期期數設定，對應PayType Table 中的 PayType0rateNum
            getInstallmentInfo.PayType0rateNum = (int)payType.PayType0rateNum;
            // 分期利率
            getInstallmentInfo.InsRate = insRate;
            // 該購物車中總折價金額
            getInstallmentInfo.CouponPrice = totalCouponPrice;
            // 該購物車中滿額贈折扣總金額
            getInstallmentInfo.DiscountAmount = totalDiscountAmount;
            // 該購物車中總利息金額
            getInstallmentInfo.TotalInsRateFees = insRateFees;
            // 該購物車中未扣除折價金額與增加利息前的原始總金額
            getInstallmentInfo.OriginalPriceSum = cartPriceSum;
            // 該購物車扣除折價金額、滿額贈折價金額與增加利息後的新總金額
            getInstallmentInfo.NewPriceSum = cartPriceSum + insRateFees - totalCouponPrice - totalDiscountAmount;
            // 是否成功執行利息金額儲存
            getInstallmentInfo.SaveSuccess = true;
            try
            {
                // 儲存資訊
                db_before.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Info("購物車SalesOrderGroupID[" + salesOrderGroupID + "] 利息分配失敗! [ErrorMessage] " + e.Message + " [StackTrace] " + e.StackTrace);
                getInstallmentInfo.SaveSuccess = false;
            }

            return getInstallmentInfo;
        }

        /// <summary>
        /// 測試用
        /// </summary>
        public void GetCode()
        {
            WebRequest myWebRequest = WebRequest.Create("http://www.ncbi.nlm.nih.gov/nuccore/NC_009930.1?report=fasta&log$=seqview&format=text&from=13910&to=14409");
            myWebRequest.Timeout = 10000;
            myWebRequest.Method = "GET";
            string strHtml = String.Empty;
            using (WebResponse myWebResponse = myWebRequest.GetResponse())
            {
                using (Stream myStream = myWebResponse.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(myStream))
                    {
                        strHtml = myStreamReader.ReadToEnd();
                    }
                }
            }

            int first = strHtml.IndexOf("<pre>");
            int last = strHtml.IndexOf("</pre>");
            string getDNA = strHtml.Substring(first + 5, last - first - 6);
        }

        [HttpPost]
        public bool LBSMoneyCompute(string itemId, int qty, string accountId, string payTypeId)
        {
            TWSqlDBContext db = new TWSqlDBContext();
            ItemDisplayPrice itemDisplayPrice = db.ItemDisplayPrice.Where(p => p.ItemID == Convert.ToInt16(itemId)).FirstOrDefault();
            decimal displayPrice = itemDisplayPrice.ItemCostTW;
            decimal couponPrice = 500;//先假設為500
            return false;
        }
    }
}
