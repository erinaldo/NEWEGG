using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.Models.Logistics.HiLife
{

    /// <summary>
    /// 取貨完成檔
    /// </summary>
    public class HiLifeF05
    {
        private TWNewEgg.DB.Models.Logistics.HiLife.F05Head m_objHead = null;
        private List<TWNewEgg.DB.Models.Logistics.HiLife.F05Body> m_listBody = null;
        private TWNewEgg.DB.Models.Logistics.HiLife.F05Tail m_objTail = null;

        public HiLifeF05()
        {
        }
        public F05Head head { get; set; }
        public List<F05Body> body { get; set; }
        public F05Tail tail { get; set; }
    }

    /// <summary>
    ///  Hi-Life F05的Head
    /// </summary>
    [Table("HiLifeF05Head")]
    public class F05Head
    {
        public F05Head()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        ///  區別碼, 固定回傳 HiLifeFormat.Head, 值為1
        /// </summary>
        public string Rdfmt { get; set; }

        /// <summary>
        /// 送件者代號, 長度3位元
        /// </summary>
        public string Sncd { get; set; }

        /// <summary>
        /// 處理日期, 長度8位元
        /// </summary>
        public string Prdt { get; set; }

        /// <summary>
        /// 備用, 長度37位元, 固定放0
        /// </summary>
        public string Fil { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

    }

    /// <summary>
    ///  Hi-Life F05的Body
    /// </summary>
    [Table("HiLifeF05Body")]
    public class F05Body
    {
        public F05Body()
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
        /// 區別碼, 固定回傳HiLiftFormat.Body的值, 值為2
        /// </summary>
        public string Rdfmt { get; set; }

        /// <summary>
        /// 取貨通路代碼, 長度3位元
        /// </summary>
        public string Cnno { get; set; }

        /// <summary>
        /// 第一段條碼, 長度9位元
        /// </summary>
        public string Bc1 { get; set; }

        /// <summary>
        /// 第二段條碼, 長度16位元
        /// </summary>
        public string Bc2 { get; set; }

        /// <summary>
        /// 取貨門市編號, 長度7位元
        /// </summary>
        public string Stno { get; set; }

        /// <summary>
        /// 繳費日期, 長度8位元
        /// </summary>
        public string Rtdt { get; set; }

        /// <summary>
        /// 實繳金額, 長度5位元, 不足5位元, 左側補齊0
        /// </summary>
        public string Realamt { get; set; }

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
    ///  Hi-Life F05的Tail
    /// </summary>
    [Table("HiLifeF05Tail")]
    public class F05Tail
    {
        public F05Tail()
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
        /// 區別碼, 固定回傳HiLiftFormat.Tail的值, 值為3
        /// </summary>
        public string Rdfmt { get; set; }

        /// <summary>
        /// 總筆數, 長度8位元, 不含Head和Tail之筆數, 不足8位元, 左側補齊0
        /// </summary>
        public string Rdcnt { get; set; }

        /// <summary>
        /// 總金額, 長度13位元, 不足13位元, 左側補齊0
        /// </summary>
        public string Amt { get; set; }

        /// <summary>
        /// 備用, 長度27位元, 固定放0
        /// </summary>
        public string Fil2 { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

    }
}
