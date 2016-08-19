using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Discard4
{
    /// <summary>
    /// 癈四機回收四聯單
    /// </summary>
    public class Discard4ItemVM
    {

        /// <summary>
        /// 系統流水編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 購車商品編號
        /// </summary>
        public string SalesorderCode { get; set; }

        /// <summary>
        /// 購車數量編號
        /// </summary>
        public string SalesorderitemCode { get; set; }

        /// <summary>
        /// 賣場品編號
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 安裝日期
        /// </summary>
        public DateTime? InstalledDate { get; set; }

        /// <summary>
        /// 回收四聯單號
        /// </summary>
        public string NumberCode { get; set; }

        /// <summary>
        /// 回收狀態
        /// ''=未處理, 1=回收, 0=不回收
        /// </summary>
        public string Discard4Flag { get; set; }

        /// <summary>
        /// 創建者
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }
       

    }
}
