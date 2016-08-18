using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("eventfile")]
    public class EventFile
    {
        public EventFile()
        {
        }

        /// <summary>
        /// 檔案序號
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        /// <summary>
        /// 對應的Event Id
        /// </summary>
        public int eventid { get; set; }

        /// <summary>
        /// 副檔名, 如 ".txt"
        /// </summary>
        public string subfilename { get; set; }
        /// <summary>
        /// 建檔時間
        /// </summary>
        public DateTime createdate { get; set; }
        /// <summary>
        /// 建檔人
        /// </summary>
        public string createuser { get; set; }
        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime? updatedate { get; set; }
        /// <summary>
        /// 最後修改人
        /// </summary>
        public string updateuser { get; set; }
    }//end class
}
