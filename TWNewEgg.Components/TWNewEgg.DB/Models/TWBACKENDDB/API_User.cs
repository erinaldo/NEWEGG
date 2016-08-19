using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("API_User")]
    public class API_User
    {
        public API_User()
        {
            SetEnable(false);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string UserEmail { get; set; }
        /// <summary>
        /// Y = Enable, N = Disable
        /// </summary>
        public string Enable { get; set; }
        public string AccessToken { get; set; }
        public string AuthKey { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public System.DateTime InDate { get; set; }
        public int InUserID { get; set; }

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
