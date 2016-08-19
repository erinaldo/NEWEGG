using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.Discard4
{
    /// <summary>
    /// 癈四機同意
    /// </summary>
    public class Discard4DM
    {

        /// <summary>
        /// 系統流水編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 購物車編號
        /// </summary>
        public int SalesOrderGroupID { get; set; }


        /// <summary>
        /// 同意癈四機回收
        /// Y=同意, 預設NULL
        /// </summary>
        public string AgreedDiscard4 { get; set; }

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
