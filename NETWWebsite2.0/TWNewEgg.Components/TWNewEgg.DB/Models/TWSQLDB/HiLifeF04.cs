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
    /// 預計進店檔
    /// </summary>
    public class HiLifeF04
    {
        private TWNewEgg.DB.Models.Logistics.HiLife.F04Head m_objHead = null;
        private List<TWNewEgg.DB.Models.Logistics.HiLife.F04Body> m_listBody = null;
        private TWNewEgg.DB.Models.Logistics.HiLife.F04Tail m_objTail = null;

        public HiLifeF04()
        {
        }
        public F04Head head { get; set; }
        public List<F04Body> body { get; set; }
        public F04Tail tail { get; set; }

    }

    /// <summary>
    /// Hi-Life F04的表頭
    /// </summary>
    [Table("HiLifeF04Head")]
    public class F04Head
    {
        public F04Head()
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
    /// Hi-Life F04的Body
    /// </summary>
    [Table("HiLifeF04Body")]
    public class F04Body
    {
        public F04Body()
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
        /// 取貨門市編號, 長度7位元, 不足7位元右側補齊空白, 超過7自動截斷
        /// </summary>
        public string Stno { get; set; }

        /// <summary>
        /// EC 訂單編號, 長度11位元, 超過11自動截斷
        /// </summary>
        public string Odno { get; set; }

        /// <summary>
        /// 商品進店日, 長度8位元, 超過8自動截斷
        /// </summary>
        public string Dcstdt { get; set; }

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
    /// Hi-Life F04的Tail
    /// </summary>
    [Table("HiLifeF04Tail")]
    public class F04Tail
    {
        public F04Tail()
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
