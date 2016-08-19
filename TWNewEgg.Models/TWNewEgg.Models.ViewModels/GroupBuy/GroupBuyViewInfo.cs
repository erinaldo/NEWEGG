using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.GroupBuy
{
    public class GroupBuyViewInfo
    {
        public int ID { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsExpired { get; set; }
        public bool IsSoldOut { get; set; }
        public bool IsShowNew { get; set; }
        public bool IsShowHot { get; set; }
        public bool IsShowExclusive { get; set; }
        public bool IsShowNeweggUSASync { get; set; }
        public string Title { get; set; }
        public string PromoText { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string OriginalPrice { get; set; }
        public string GroupBuyPrice { get; set; }
        public string Discount { get; set; }
        public string DiscountPercentage { get; set; }
        public string SalesOrderCount { get; set; }
        public string ItemLinkButtonImageUrl { get; set; }
        public string ItemLinkButtonText { get; set; }
        public string ItemLink { get; set; }
        public string SellQuantity { get; set; }
        public string Sdesc { get; set; }
        public string AdCopy { get; set; }
        public string ImgUrl { get; set; }
        public int ItemID { get; set; }
        private string disableImageUrl = "/img/groupbuy/gbBuyDisable.png"; //disable 前往 Button Image
        private string enableImageUrl = "/img/groupbuy/gbBuy.png"; //enable 前往 Button Image
        private string endImageUrl = "/img/groupbuy/gbEnd.png"; //結束 Button Image
        private string soldOutImageUrl = "/img/groupbuy/gbSoldOut.png"; //結束 Button Image
        /// <summary>
        /// Get GroupBuyDisableImage
        /// </summary>
        /// <value>string</value>
        public string GroupBuyDisableImage
        {
            get
            {
                return this.disableImageUrl;
            }
        }

        /// <summary>
        /// Get GroupBuyEnableImage
        /// </summary>
        /// <value>string</value>
        public string GroupBuyEnableImage
        {
            get
            {
                return this.enableImageUrl;
            }
        }

        /// <summary>
        /// Get GroupBuyEndImage
        /// </summary>
        /// <value>string</value>
        public string GroupBuyEndImage
        {
            get
            {
                return this.endImageUrl;
            }
        }

        /// <summary>
        /// Get GroupBuySoldOutImage
        /// </summary>
        /// <value>string</value>
        public string GroupBuySoldOutImage
        {
            get
            {
                return this.soldOutImageUrl;
            }
        }

    }
}
