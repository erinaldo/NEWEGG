using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 用於任選館的麵包屑使用.
    /// </summary>
    public class Breadcrumbs
    {
        /// <summary>
        /// 項目顯示文字.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 對應的分類ID
        /// </summary>
        public int CategoryID { get; set; }
        
        /// <summary>
        /// 下拉項目清單
        /// </summary>
        public List<BreadcrumbItem> DropDownItems { get; set; }

        /// <summary>
        /// 所有的父階Category
        /// </summary>
        public List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem> ListParentCategories { get; set; }
    }
}
