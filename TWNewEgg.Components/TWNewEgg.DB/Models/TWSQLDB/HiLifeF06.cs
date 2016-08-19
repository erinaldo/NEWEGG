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
    /// 門市預退檔
    /// </summary>
    public class HiLifeF06
    {
        private TWNewEgg.DB.Models.Logistics.HiLife.F06Head m_objHead = null;
        private List<TWNewEgg.DB.Models.Logistics.HiLife.F06Body> m_listBody = null;
        private TWNewEgg.DB.Models.Logistics.HiLife.F06Tail m_objTail = null;

        public HiLifeF06()
        {
        }
        public F06Head head { get; set; }
        public List<F06Body> body { get; set; }
        public F06Tail tail { get; set; }
    }

    /// <summary>
    /// Hi-Life F06的Head
    /// </summary>
    [Table("HiLifeF06Head")]
    public class F06Head
    {
        public F06Head()
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
        /// 備用, 長度24位元, 固定放0
        /// </summary>
        public string Fil { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// Hi-Life F06的Body
    /// </summary>
    [Table("HiLifeF06Body")]
    public class F06Body
    {
        public F06Body()
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
        /// 區別碼, 固定回傳HiLiftFormat.Body的值, 值為2
        /// </summary>
        public string Rdfmt { get; set; }

        /// <summary>
        /// 退貨類型, 值為1, 表示第一次預退, 值為2, 表示第二次預退
        /// </summary>
        public string Retype { get; set; }

        /// <summary>
        /// 網站代號, 長度3位元
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
        /// 店鋪預定退貨日, 長度8位元
        /// </summary>
        public string Scrtdt { get; set; }

        /// <summary>
        /// 商品實際金額, 長度5位元, 含稅正整數, 不足5位元, 左側補齊0
        /// </summary>
        public string Realamt { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// Hi-Life F05的Tail
    /// </summary>
    [Table("HiLifeF06Tail")]
    public class F06Tail
    {
        public F06Tail()
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
        /// 總金額, 長度7位元, 商品實際金額總計, 不足7位元, 左側補齊0
        /// </summary>
        public string Realamt { get; set; }

        /// <summary>
        /// 備用, 長度20位元, 固定放0
        /// </summary>
        public string Fil2 { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }
}
