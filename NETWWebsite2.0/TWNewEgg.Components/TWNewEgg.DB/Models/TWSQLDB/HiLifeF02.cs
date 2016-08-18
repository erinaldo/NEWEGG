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
    /// 廠商出貨檔
    /// </summary>
    public class HiLifeF02
    {
        private TWNewEgg.DB.Models.Logistics.HiLife.F02Head m_objHead = null;
        private List<TWNewEgg.DB.Models.Logistics.HiLife.F02Body> m_listBody = null;
        private TWNewEgg.DB.Models.Logistics.HiLife.F02Tail m_objTail = null;

        public HiLifeF02()
        { 
        }

        public F02Head head { get; set; }
        public List<F02Body> body { get; set; }
        public F02Tail tail { get; set; }
    }

    /// <summary>
    /// Hi-Life F02的表頭
    /// </summary>
    [Table("HiLifeF02Head")]
    public class F02Head
    {
        public F02Head()
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
    /// Hi-Life F02的Body
    /// </summary>
    [Table("HiLifeF02Body")]
    public class F02Body
    {
        public F02Body()
        { 
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 對應的表頭Id
        /// </summary>
        public int? HeadId { get; set; }

        /// <summary>
        /// 區別碼, Body固定回傳HiLiftFormat.Body的值, 值為2
        /// </summary>
        public string Rdfmt { get; set; }

        /// <summary>
        /// EC 網站代號, 長度3位元, 超過3位元自動截斷
        /// </summary>
        public string Ecno { get; set; }

        /// <summary>
        /// 取貨門市編號, 長度7位元, 不足7位元右側補齊空白, 超過7自動截斷
        /// </summary>
        public string Stno { get; set; }

        /// <summary>
        /// 取貨店名, 中文, 長度18位元, 不足18位元右側補齊空白, 超過18自動截斷
        /// </summary>
        public string Stnm { get; set; }

        /// <summary>
        /// EC 訂單編號, 長度11位元, 超過11自動截斷
        /// </summary>
        public string Odno { get; set; }

        /// <summary>
        /// 取貨人姓名, 長度20位元, 不足20位元右側補齊空白, 超過20自動截斷
        /// </summary>
        public string Cutknm { get; set; }

        /// <summary>
        /// 路線路順, 長度5位元, 超過5自動截斷
        /// </summary>
        public string Dcrono { get; set; }

        /// <summary>
        /// 廠商代號, 長度3位元, 超過3自動截斷
        /// </summary>
        public string Edcno { get; set; }

        /// <summary>
        /// 第一段條碼, 長度9位元, 超過9自動截斷
        /// </summary>
        public string Bc1 { get; set; }

        /// <summary>
        /// 第二段條碼, 長度16位元, 超過16自動截斷
        /// </summary>
        public string Bc2 { get; set; }

        /// <summary>
        /// 商品實際金額, 長度5位元, 不足5位元左側補齊0, 超過5自動截斷
        /// </summary>
        public string Realamt { get; set; }

        /// <summary>
        /// 商品類型,0 為一般商品,1 為票券商品
        /// </summary>
        public string Prodnm { get; set; }

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
    /// Hi-Life F02的Tail
    /// </summary>
    [Table("HiLifeF02Tail")]
    public class F02Tail
    {
        public F02Tail()
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
        public string Realamt { get; set; }

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
