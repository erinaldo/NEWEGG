using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Models.DBModels.TWSQLDBExtModels
{
    public class DbSOInfo
    {
        public enum SOInfoStatus
        {
            初始 = 0,
            已付款 = 1,
            未付款 = 2,
            失敗 = 3,
            訂單成立 = 10,

            待出貨 = 11,
            海外轉運中 = 20,
            空運中 = 21,
            清關中 = 22,
            配送中 = 23,
            已送達 = 30,
            已退款 = 40,
            已退貨 = 41,
            錯誤 = 99,
            退款中 = 100,
            退貨中 = 101,
            採購單確認中 = 102
        }
        public SalesOrder Main { get; set; }
        public PurchaseOrder PO { get; set; }
        public List<SalesOrderItem> SOItems { get; set; }
        public SOInfoStatus Status { get; set; }
    }
}
