using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemGroupDetailProperty
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> ItemTempID { get; set; }
        public int SellerID { get; set; }

        /// <summary>
        /// Master Property Id
        /// </summary>
        public int? MasterPropertyID { get; set; }

        /// <summary>
        /// Second Property Id
        /// </summary>
        public int PropertyID { get; set; }

        /// <summary>
        /// Master Property Group Id
        /// </summary>
        public int GroupValueID { get; set; }

        /// <summary>
        /// Second Property Group Id
        /// </summary>
        public int ValueID { get; set; }

        /// <summary>
        /// Second  Custom Display Value
        /// </summary>
        public string ValueName { get; set; }

        /// <summary>
        /// Master Custom Display Value
        /// </summary>
        public string InputValue { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int InUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}