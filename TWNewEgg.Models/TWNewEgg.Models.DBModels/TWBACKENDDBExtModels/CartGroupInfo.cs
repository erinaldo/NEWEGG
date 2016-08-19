using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.Models.DBModels.TWBACKENDDBExtModels
{
    public class CartGroupInfo
    {
        public enum DataTypeEnum
        {
            XQ,
            XD_Offline,
            XD_PayOnDelivery,
            XI,
            XI_OverSeaBuyOutUSD,
            XIRMA
        }

        public int SalesOrderGroupID { get; set; }
        /// <summary>
        /// 會計文件日期
        /// </summary>
        public DateTime DocDate { get; set; }

        /// <summary>
        /// 符合條件的訂單
        /// </summary>
        public IEnumerable<Cart> SalesOrderList { get; set; }

        public DataTypeEnum DataType { get; set; }
    }

    public class PurchaseOrderItemGroupInfo
    {
        public string SalesOrderGroupID { get; set; }
        /// <summary>
        /// 會計文件日期
        /// </summary>
        public DateTime DocDate { get; set; }

        /// <summary>
        /// 符合條件的訂單
        /// </summary>
        public IEnumerable<PurchaseOrderitemTWBACK> PurchaseOrderItemList { get; set; }

        //public DataTypeEnum DataType { get; set; }
    }
}
