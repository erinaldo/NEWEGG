using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class TransactionSPResult
    {
        [DisplayName("日期")]
        public string Date { get; set; }

        [DisplayName("交易類別")]
        public string TransactionType { get; set; }
        
        [DisplayName("訂單編號")]
        public string CartID { get; set; }
        
        [DisplayName("發票編號")]
        public string InvoiceNo { get; set; }
        
        [DisplayName("商家商品編號")]
        public string SellerProductID { get; set; }

        [DisplayName("Newegg Item #")]
        public int NeweggItemNo { get; set; }
        
        [DisplayName("商品描述")]
        public string ProductDescription { get; set; }

        [DisplayName("商品成色")]
        public string ProductStatus { get; set; }
        
        [DisplayName("金額($)")]
        public decimal Price { get; set; }
        
        [DisplayName("運費")]
        public decimal ShippingFee { get; set; }
        
        [DisplayName("傭金費($)")]
        public decimal CommissionFee { get; set; }
     
        [DisplayName("結算編號")]
        public string SettlementID { get; set; }
       
        


        
    }
}
