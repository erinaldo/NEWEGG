using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TWNewEgg.Models.DomainModels.Advertising
{
    class AdLayer3ItemDM
    {
       
        public int AdLayer3ID { get; set; }
        
        public int ItemID { get; set; }
        /// <summary>
        /// 顯示排序，1開始
        /// </summary>
        public int Showorder { get; set; }
        /// <summary>
        /// 0：不顯示；1：顯示
        /// </summary>
        public int ShowAll { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
