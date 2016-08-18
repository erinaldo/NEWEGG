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
    /// 店鋪資料檔
    /// </summary>
    public class HiLifeF01
    {
        public HiLifeF01()
        {
        }

        public F01Head head { get; set; }
        public List<F01Body> body { get; set; }
        public F01Tail tail { get; set; }


    }

    /// <summary>
    /// Hi-Life F01的表頭
    /// </summary>
    [Table("HiLifeF01Head")]
    public class F01Head
    {
        public F01Head()
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
        /// 送件者代號
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
    /// Hi-Life F01的Body
    /// </summary>
    [Table("HiLifeF01Body")]
    public class F01Body 
    {
        public F01Body()
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
        /// 取貨店號, 長度7碼, 不足7碼右側補齊空白, 超過7碼自動截斷
        /// </summary>
        public string Stno { get; set; }

        /// <summary>
        /// 店鋪名稱, 長度18碼, 不足18碼右側補齊空白, 超過18碼自動截斷
        /// </summary>
        public string Stnm { get; set; }

        /// <summary>
        /// 店鋪電話, 長度20, 不足20右側補齊空白, 超過20碼自動截斷
        /// </summary>
        public string Sttel { get; set; }

        /// <summary>
        /// 店鋪縣市, 中文, 長度10位元, 不足10位元右測補齊空白, 超過10自動截斷
        /// </summary>
        public string Stcity { get; set; }
        public string Stcntry { get; set; }
        public string Stadr { get; set;} 
        public string Zipcd { get; set; }
        public string Dcrono { get; set; }
        public string Sdate { get; set; }
        public string Edate { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }

    [Table("HiLifeF01Tail")]
    public class F01Tail 
    {

        public F01Tail()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 對應的表頭Id
        /// </summary>
        public int HeadId { get; set; }

        public string Rdfmt { get; set; }

        public string Rdcnt { get; set; }
        public string Fil2 { get; set; }

        /// <summary>
        /// 建記錄的日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
    }


}
