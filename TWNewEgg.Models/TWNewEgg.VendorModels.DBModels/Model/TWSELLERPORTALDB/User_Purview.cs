using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("User_Purview")]
    public partial class User_Purview
    {
        public User_Purview()
        {
            SetEnable(false);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }

        [Key, Column(Order = 0)]
        //[ForeignKey("ActionID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FunctionID { get; set; }

        [Key, Column(Order = 1)]
        //[ForeignKey("UserID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }
        /// <summary>
        /// Y = Enable, N = Disable
        /// </summary>
        public string Enable { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }

        public bool IsEnable()
        {
            return this.Enable.ToLower() == "y";
        }
        public void SetEnable(bool flag)
        {
            if (flag)
                this.Enable = "Y";
            else
                this.Enable = "N";
        }
    }
}
