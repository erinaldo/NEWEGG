using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    /// <summary>
    /// SellerPortal ItemCreation View Model
    /// </summary>
    public class SPItemCreation
    {
        public SPItemCreation()
        {
            this.ProductsInfo = new List<ProductsInfoResult>();
            this.ItemInfos = new List<ItemInfoResult>();
        }

        public List<ProductsInfoResult> ProductsInfo { get; set; }

        public List<ItemInfoResult> ItemInfos { get; set; }
    }
}
