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
    /// 物流取消出貨檔
    /// </summary>
    public class HiLifeF09
    {
        public HiLifeF09()
        {
        }
        public F09Head head { get; set; }
        public List<F09Body> body { get; set; }
        public F09Tail tail { get; set; }

    }
    /// <summary>
    ///  Hi-Life F09的Head
    /// </summary>
    [Table("HiLifeF09Head")]
    public class F09Head
    {

        public F09Head()
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
        /// 備用, 長度6位元, 固定放0
        /// </summary>
        public string Fil { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    ///  Hi-Life F09的Body
    /// </summary>
    [Table("HiLifeF09Body")]
    public class F09Body
    {
        public F09Body()
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
        /// 取消類型, 長度3位元
        /// </summary>
        public string Ret_m { get; set; }

        /// <summary>
        /// 廠商代號, 長度3位元
        /// </summary>
        public string Ecno { get; set; }

        /// <summary>
        /// 訂單編號, 長度11位元
        /// </summary>
        public string Odno { get; set; }

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
    ///  Hi-Life F09的Tail
    /// </summary>
    [Table("HiLifeF09Tail")]
    public class F09Tail
    {
        public F09Tail()
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
        /// 備用, 長度9位元, 固定放0
        /// </summary>
        public string Fil2 { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }
}
