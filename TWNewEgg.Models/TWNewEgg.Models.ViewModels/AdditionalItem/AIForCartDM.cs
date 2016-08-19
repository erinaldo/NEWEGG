using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.AdditionalItem
{
   public class AIForCartDM
    {
        /// <summary>
        /// Related to Status
        /// </summary>
        public enum AdditionalItemStatus
        {
            Planning = -1,
            Disable = 0,
            Enable = 1
        };
        /// <summary>
        /// Related to Specific
        /// </summary>
        public enum SpecificStatus
        {
            AllAccount = 0,
            NeweggAccount = 1,
            VIPAccount = 2
        };
        /// <summary>
        /// Related to CartType
        /// </summary>
        public enum CartTypeStatus
        {
            Domestic = 0,
            Internation = 1,
            ChooseAny = 2
        };
        public int ID { get; set; }

        public int ItemID { get; set; }
        /// <summary>
        /// Related to AdditionalItemStatus
        /// </summary>
     
        public int Status { get; set; }

        public decimal LimitedPrice { get; set; }

        public Nullable<int> ItemGroupID { get; set; }
        /// <summary>
        /// Related to SpecificStatus
        /// </summary>

        public int Specific { get; set; }
   
        public int StartDate { get; set; }
  
        public int EndDate { get; set; }
        /// <summary>
        /// Related to CartTypeStatus
        /// </summary>
  
        public int CartType { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
