using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.ViewModels.Category
{
    /// <summary>
    /// 任選館的單一商品, 附加已選取數量.
    /// </summary>
    public class OptionStoreItemCell_View : OptionStoreItemCell
    {
        public OptionStoreItemCell_View()
        { }

        public OptionStoreItemCell_View(OptionStoreItemCell ItemCell)
        {
            this.ItemID = ItemCell.ItemID;
            this.ItemImage = ItemCell.ItemImage;
            this.LogoImage = ItemCell.LogoImage;
            this.Title = ItemCell.Title;
            this.UnitPrice = ItemCell.UnitPrice;
            this.Url = ItemCell.Url;
            this.PickQty = ItemCell.PickQty;
        }

    }
}
