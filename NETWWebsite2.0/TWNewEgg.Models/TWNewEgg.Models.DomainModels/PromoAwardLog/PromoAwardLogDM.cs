using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PromoAwardLog
{
    public class PromoAwardLogDM
    {
        public int ID { get; set; }
        public int AccountID { get; set; }
        public string Email { get; set; }
        public string ChName { get; set; }
        public string EngFirstName { get; set; }
        public string EngLastName { get; set; }
        public int PromoActiveID { get; set; }
        public string AwardName { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }

        /// <summary>
        /// 活動名稱
        /// </summary>
        public string activityName { get; set; }
    }
}
