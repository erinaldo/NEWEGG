using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("Seller_Action")]
    public class Seller_Action
    {
        public Seller_Action()
        {
            SetEnable(false);
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionID { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ActionDescription { get; set; }
        public int FunctionID { get; set; }
        public string FNActiveKey { get; set; }
        public string HttpMethod { get; set; }
        /// <summary>
        /// Y = Enable, N = Disable
        /// </summary>
        public string Enable { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public System.DateTime InDate { get; set; }
        public int InUserID { get; set; }
        public string FNCategoryName { get; set; }
        public string FunctionName { get; set; }
    
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
