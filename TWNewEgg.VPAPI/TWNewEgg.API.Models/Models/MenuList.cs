using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class MenuList
    {
        public MenuList()
        {
            this.listOptions = new Dictionary<string, string>();
        }

        /// <summary>
        /// 主選項名稱
        /// </summary>
        public string mainOption { get; set; }

        /// <summary>
        /// 子選項:key:子選項名稱, value:子選項url
        /// </summary>
        public Dictionary<string, string> listOptions { get; set; }
    }

    public class MenuPurviewInfo
    {
        public string PurviewType { get; set; }
        public int UserID { get; set; }
        public int? SellerID { get; set; }
        public int GroupID { get; set; }
    }
}
