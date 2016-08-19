using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Store
{
    /// <summary>
    /// 後台IPP的類別基本資料管理
    /// 基本資料下面的任選館
    /// </summary>
    public class SubCategory_OptionStore_VM
    {
        public enum ConstShowAll
        {
            Hide = 0,
            Show = 1
        }

        public int ID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Title { get; set; }
        public Nullable<bool> IsFormat { get; set; }
        public Nullable<decimal> FreeCost { get; set; }
        public Nullable<int> SellerID { get; set; }
        // ConstShowAll
        public Nullable<int> ShowAll { get; set; }
        public Nullable<int> Showorder { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }

        /// <summary>
        /// 是否為規格品
        /// </summary>
        public int IsSpecOption { get; set; }

        /// <summary>
        /// Verification InputData
        /// </summary>
        public string strNullErrorMsg { get; set; }
        public string strTpyeErrorMsg { get; set; }
        public string strLessZeroErrorMsg { get; set; }
        public string strDBOverflowErrorMsg { get; set; }
        public string strEndLessStartDateErrorMsg { get; set; }

    }
}
