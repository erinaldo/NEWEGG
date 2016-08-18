using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.Models.DBModels.TWBACKENDDBExtModels
{
    public class SapPriceInfo
    {
        public SapPriceInfo() { }

        //public SapPriceInfo(AccountsDocumentType.DocTypeEnum docType, int shipType, IQueryable<ItemInStock_trans> itemInStockTransList, IQueryable<Process> procList)
        public SapPriceInfo(AccountsDocumentType.DocTypeEnum docType, int shipType, List<ItemInStock_trans> itemInStockTransList, List<Process> procList)
        {
            decimal itemPrice = 0;

            //this.ShippingExpense = procList.Sum(x => (decimal)x.ShippingExpense);
            //this.ServiceExpense = procList.Sum(x => (decimal)x.ServiceExpense);
            //this.TaxExpense = procList.Sum(x => (decimal)x.Tax);
            //this.ApportionedAmount = procList.Sum(x => (x.Pricecoupon ?? 0) + x.ApportionedAmount);
            //this.GoodsPrice = procList.Sum(x => x.Price ?? 0);

            foreach (Process procInfo in procList)
            {
                this.ShippingExpense += (decimal)procInfo.ShippingExpense;
                this.ServiceExpense += (decimal)procInfo.ServiceExpense;
                this.TaxExpense += (decimal)procInfo.Tax;
                this.ApportionedAmount += ((procInfo.Pricecoupon ?? 0) + procInfo.ApportionedAmount);
                this.GoodsPrice += (procInfo.Price ?? 0);
            }

            switch (shipType)
            {
                case 3:
                    //發票金額-三角(含稅)
                    this.InvoiceAmount = this.TaxExpense + this.ShippingExpense + this.ServiceExpense - this.ApportionedAmount;
                    this.SalesAmount = this.GoodsPrice + this.ShippingExpense + this.ServiceExpense - this.ApportionedAmount;

                    this.ImportTaxExpense = Math.Round(this.TaxExpense / (decimal)1.05, 0, MidpointRounding.AwayFromZero);
                    this.ImportTax = this.TaxExpense - this.ImportTaxExpense;

                    this.BusinessShippingExpense = Math.Round((this.ShippingExpense - this.ApportionedAmount) / (decimal)1.05, 0, MidpointRounding.AwayFromZero);
                    this.BusinessServiceExpense = Math.Round(this.ServiceExpense / (decimal)1.05, 0, MidpointRounding.AwayFromZero);
                    break;
                default:
                    //存貨-未稅價、PO單-含稅價

                    //三角無存貨資料
                    if (itemInStockTransList != null && itemInStockTransList.Count > 0)
                    {
                        //成本
                        this.PreAVGCost = Math.Floor(itemInStockTransList.Select(x => x.Cost).Sum()) - (int)Math.Floor(Math.Abs(itemInStockTransList.Select(x => x.PreTotalShippingFee - x.AfterTotalShippingFee).Sum() ?? 0)) - (int)Math.Floor(Math.Abs(itemInStockTransList.Select(x => x.PreTotalTaxandDuty - x.AfterTotalTaxandDuty).Sum() ?? 0)) - (int)Math.Floor(Math.Abs(itemInStockTransList.Select(x => x.PreTotalServiceCharges - x.AfterTotalServiceCharges).Sum() ?? 0)) - (int)Math.Floor(Math.Abs(itemInStockTransList.Select(x => x.PreTotalCustomsCharges - x.AfterTotalCustomsCharges).Sum() ?? 0));
                        //運費
                        this.PreAVGCostShipandTax = Math.Floor(Math.Abs(itemInStockTransList.Select(x => x.PreTotalShippingFee - x.AfterTotalShippingFee).Sum() ?? 0));
                        //關稅
                        this.PreAVGCostTaxandDuty = Math.Floor(Math.Abs(itemInStockTransList.Select(x => x.PreTotalTaxandDuty - x.AfterTotalTaxandDuty).Sum() ?? 0)) + (int)Math.Floor(Math.Abs(itemInStockTransList.Select(x => x.PreTotalServiceCharges - x.AfterTotalServiceCharges).Sum() ?? 0));
                        //報關費
                        this.PreAVGCostCustoms_Charge = Math.Floor(Math.Abs(itemInStockTransList.Select(x => x.PreTotalCustomsCharges - x.AfterTotalCustomsCharges).Sum() ?? 0));
                    }
                    else
                    {
                        //XI、XIRMA 有發票的訂單一定會有ItemInStock_trans記錄
                        switch (docType)
                        {
                            case AccountsDocumentType.DocTypeEnum.XI:
                            case AccountsDocumentType.DocTypeEnum.XIRMA:
                                throw new Exception("ItemInStock_trans查無資料。");
                        }                        
                    }

                    //發票金額(含稅)
                    foreach (var singleSOItem in procList)
                    {
                        itemPrice += singleSOItem.DisplayPrice.Value + (decimal)Math.Round(double.Parse(singleSOItem.InstallmentFee.ToString()), 0, MidpointRounding.AwayFromZero) - ((singleSOItem.Pricecoupon == null) ? 0 : singleSOItem.Pricecoupon.Value) - singleSOItem.ApportionedAmount;
                    }

                    this.InvoiceAmount = itemPrice;
                    this.SalesAmount = this.InvoiceAmount;
                    break;
            }

            this.SalesRevenue = Math.Round(this.InvoiceAmount / (decimal)1.05, 0, MidpointRounding.AwayFromZero);
            this.SalesTax = this.InvoiceAmount - this.SalesRevenue;
        }
        
        /// <summary>
        /// 運費(含稅)
        /// </summary>
        public decimal ShippingExpense { get; set; }
        /// <summary>
        /// 服務費(含稅)
        /// </summary>
        public decimal ServiceExpense { get; set; }
        /// <summary>
        /// 進口稅賦(含稅)
        /// </summary>
        public decimal TaxExpense { get; set; }
        /// <summary>
        /// 折價卷金額(含稅)
        /// </summary>
        public decimal ApportionedAmount { get; set; }
        /// <summary>
        /// 商品小計(含稅)
        /// </summary>
        public decimal GoodsPrice { get; set; }
                
        /// <summary>
        /// 存貨成本(未稅)
        /// </summary>
        public decimal PreAVGCost { get; set; }

        /// <summary>
        /// 存貨運費(未稅)
        /// </summary>
        public decimal PreAVGCostShipandTax { get; set; }

        /// <summary>
        /// 存貨關稅(未稅)
        /// </summary>
        public decimal PreAVGCostTaxandDuty { get; set; }

        /// <summary>
        /// 存貨報關費(未稅)
        /// </summary>
        public decimal PreAVGCostCustoms_Charge { get; set; }


        /// <summary>
        /// 訂單結帳金額(含稅)
        /// </summary>
        public decimal SalesAmount { get; set; }
        /// <summary>
        /// 訂單發票金額(含稅)
        /// </summary>
        public decimal InvoiceAmount { get; set; }
        /// <summary>
        /// 銷項收入(未稅)
        /// </summary>
        public decimal SalesRevenue { get; set; }
        /// <summary>
        /// 營業稅
        /// </summary>
        public decimal SalesTax { get; set; }
        
        /// <summary>
        /// 業務收入-運費收入(未稅-三角)
        /// </summary>
        public decimal BusinessShippingExpense { get; set; }
        /// <summary>
        /// 業務收入-服務費收入(未稅-三角)
        /// </summary>
        public decimal BusinessServiceExpense { get; set; }

        /// <summary>
        /// 進口稅費(未稅-三角)
        /// </summary>
        public decimal ImportTaxExpense { get; set; }
        /// <summary>
        /// 進口稅率(三角)
        /// </summary>
        public decimal ImportTax { get; set; }
    }
}
