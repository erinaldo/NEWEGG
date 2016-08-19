using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace TWNewEgg.DB.Models.Logistics.HiLife
{

    /// <summary>
    /// 寄件訂單檔
    /// </summary>
    public class HiLifeF03
    {
        private TWNewEgg.DB.Models.Logistics.HiLife.F03Head m_objHead = null;
        private List<TWNewEgg.DB.Models.Logistics.HiLife.F03Body> m_listBody = null;
        private TWNewEgg.DB.Models.Logistics.HiLife.F03Tail m_objTail = null;

        public HiLifeF03()
        {
        }

        public F03Head head { get; set; }
        public List<F03Body> body { get; set; }
        public F03Tail tail { get; set; }
    }

    /// <summary>
    /// Hi-Life F03的表頭
    /// </summary>
    [Table("HiLifeF03Head")]
    public class F03Head
    {
        public F03Head()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 區別碼, 固定回傳 HiLifeFormat.Head, 值為1
        /// </summary>
        public string Rdfmt { get; set; }

        /// <summary>
        /// 送件者代號, 長度3位元
        /// </summary>
        public string Sncd { get; set; }

        /// <summary>
        /// 處理日期
        /// </summary>
        public string Prdt { get; set; }

        /// <summary>
        /// 備用, 固定放0
        /// </summary>
        public string Fil { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

    }

    /// <summary>
    /// Hi-Life F03的Body
    /// </summary>
    [Table("HiLifeF03Body")]
    public class F03Body
    {
        public F03Body()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 對應的表頭Id
        /// </summary>
        public int HeadId { get; set; }

        /// <summary>
        /// 區別碼, Body固定回傳HiLiftFormat.Body的值, 值為2
        /// </summary>
        public string Rdfmt { get; set; }

        /// <summary>
        /// EC 網站代號, 長度3位元, 超過3位元自動截斷
        /// </summary>
        public string Ecno { get; set; }

        /// <summary>
        /// 取貨通路代號, 長度3位元, 超過3位元自動截斷
        /// </summary>
        public string Cnno { get; set; }

        /// <summary>
        /// 寄件門市編號, 長度7位元, 不足7位元右側補齊空白, 超過7自動截斷
        /// </summary>
        public string Stno { get; set; }

        /// <summary>
        /// EC 訂單編號, 長度11位元, 超過11位元自動截斷
        /// </summary>
        public string Odno { get; set; }

        /// <summary>
        /// 應收金額, 長度5位元, 不足5位元右側補齊空白, 超過5自動截斷
        /// </summary>
        public string Amt { get; set; }

        /// <summary>
        /// 取貨人姓名, 長度20位元, 不足20位元右側補齊空白, 超過20自動截斷
        /// </summary>
        public string Cutknm { get; set; }

        /// <summary>
        /// 取貨人行動電話, 長度20位元, 不足20位元右側補齊空白, 超過20自動截斷
        /// </summary>
        public string Cutktl { get; set; }

        /// <summary>
        /// 物流送貨車次, 長度20位元, 不足20位元右側補齊空白, 超過20自動截斷
        /// </summary>
        public string Dcrono { get; set; }

        /// <summary>
        /// 商品類型,0 為一般商品,1 為票券商品
        /// </summary>
        public string Prodnm { get; set; }

        /// <summary>
        /// 交易方式識別碼,1 為取貨付款, 3 為取貨不付款
        /// </summary>
        public string Tradetype { get; set; }

        /// <summary>
        /// 代收代號, 長度3位元, 超過3位元自動截斷
        /// </summary>
        public string Sercode { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 紀錄資料是否處理
        /// </summary>
        public int Flag { get; set; }
    }

    /// <summary>
    /// Hi-Life F03的Tail
    /// </summary>
    [Table("HiLifeF03Tail")]
    public class F03Tail
    {
        public F03Tail()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 對應的表頭Id
        /// </summary>
        public int HeadId { get; set; }

        /// <summary>
        /// 區別碼, 固定回傳 HiLifeFormat.Tail, 值為3
        /// </summary>
        public string Rdfmt { get; set; }

        /// <summary>
        /// 總筆數, 不含Head 及 Tail 之筆數, 長度8位元, 不足8位元右測補齊空白, 超過8自動截斷
        /// </summary>
        public string Rdcnt { get; set; }

        /// <summary>
        /// 總金額, 長度7位元, 不足7位元右側補齊空白, 超過7自動截斷
        /// </summary>
        public string Amt { get; set; }

        /// <summary>
        /// 備用, 固定放0
        /// </summary>
        public string Fil2 { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }
}
