using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.StoreServices.Const
{
    public class ConstCellType
    {
        /// <summary>
        /// 單張圖檔(連結)
        /// </summary>
        public const string SingleImage = "SingleImage";
        
        /// <summary>
        /// 群組連結清單
        /// </summary>
        public const string GroupList = "GroupList";
        
        /// <summary>
        /// 可滑動和連結的Logo圖集合
        /// </summary>
        public const string LogoList = "LogoList";
        
        /// <summary>
        /// 展示單一商品
        /// </summary>
        public const string Item = "Item";
        
        /// <summary>
        /// 純文字(或HTML)
        /// </summary>
        public const string Text = "Text";
        
        /// <summary>
        /// 文字疊圖(目前暫時不做)
        /// </summary>
        public const string TextOverImage = "TextOverImage";

        /// <summary>
        /// 未定義的型態(不處理,當作該Cell不存在)
        /// </summary>
        public const string Undefined = "Undefined";
    }
}
