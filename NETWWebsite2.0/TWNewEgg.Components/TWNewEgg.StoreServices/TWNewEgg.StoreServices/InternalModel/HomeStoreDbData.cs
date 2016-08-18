using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.StoreServices.InternalModel
{
    /// <summary>
    /// 這個是為了減少HomeStore的db存取次數而使用的類別, 會先一次把所有必要資料取出來存放.
    /// </summary>
    public class HomeStoreDbData
    {
        public List<WindowBlocks> Blocks { get; set; }
        public List<ItemAndSubCategoryMapping_NormalStore> Cells { get; set; }
        public List<Subcategorygroup> GroupLinks { get; set; }
        public List<Subcategorylogo> LogoImages { get; set; }
        public List<StoreItemCell> Items { get; set; }
    }
}
