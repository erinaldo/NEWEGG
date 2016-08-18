using System.Collections.Generic;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// CoreMetrics ViewData.
    /// </summary>
    public class CoreMetricsViewData
    {
        /// <summary>
        /// Gets or sets ElementTagList.
        /// </summary>
        public List<string> FireElementTagList { get; set; }

        /// <summary>
        /// Gets or sets ShopAction5 tag list.
        /// </summary>
        public List<string> ShopAction5TagList { get; set; }

        /// <summary>
        /// Gets or sets ShopAction9 tag list.
        /// </summary>
        public List<string> ShopAction9TagList { get; set; }

        /// <summary>
        /// Gets or sets ShoppingOrderInfo.
        /// </summary>
        public List<string> OrderTagList { get; set; }

        /// <summary>
        /// Gets or sets PageView Tag.
        /// </summary>
        public string PageViewTag { get; set; }
    }
}
