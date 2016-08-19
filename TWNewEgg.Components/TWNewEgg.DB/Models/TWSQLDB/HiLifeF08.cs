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
    /// 小物流驗退檔
    /// </summary>
    public class HiLifeF08
    {
        private TWNewEgg.DB.Models.Logistics.HiLife.F08Head m_objHead = null;
        private List<TWNewEgg.DB.Models.Logistics.HiLife.F08Body> m_listBody = null;
        private TWNewEgg.DB.Models.Logistics.HiLife.F08Tail m_objTail = null;

        public HiLifeF08()
        { 
        }

        public F08Head head { get; set; }
        public List<F08Body> body { get; set; }
        public F08Tail tail { get; set; }

    }
    /// <summary>
    /// Hi-Life F08的Head
    /// </summary>
    [Table("HiLifeF08Head")]
    public class F08Head
    {
        public F08Head()
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
        /// 送件者代號, 長度3位元, DLF 萊爾富文流
        /// </summary>
        public string Sncd { get; set; }

        /// <summary>
        /// 處理日期, 長度8位元
        /// </summary>
        public string Prdt { get; set; }

        /// <summary>
        /// 備用, 長度13位元, 固定放0
        /// </summary>
        public string Fil { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// Hi-Life F08的Body
    /// </summary>
    [Table("HiLifeF08Body")]
    public class F08Body
    {
        public F08Body()
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
        /// 退貨類型, 長度3位元
        /// </summary>
        public string Ret_m { get; set; }

        /// <summary>
        /// 網站代碼, 長度3位元
        /// </summary>
        public string Ecno { get; set; }

        /// <summary>
        /// 取貨門市編號, 長度7位元
        /// </summary>
        public string Stno { get; set; }

        /// <summary>
        /// 訂單編號, 長度11位元
        /// </summary>
        public string Odno { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// Hi-Life F08的Tail
    /// </summary>
    [Table("HiLifeF08Tail")]
    public class F08Tail
    {
        public F08Tail()
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
        /// 備用, 長度16位元, 固定放0
        /// </summary>
        public string Fil2 { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }
}
