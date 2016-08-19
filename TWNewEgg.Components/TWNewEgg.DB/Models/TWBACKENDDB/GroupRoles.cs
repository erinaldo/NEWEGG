using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("grouproles")]
    public class GroupRoles
    {
        [Key]
        [Display(Name = "用戶組ID")]
        public int ID { get; set; }
        [Required]
        [Display(Name = "用戶組名稱")]
        public string GroupName { get; set; }

        [Required]
        [Display(Name = "用戶組創造日期")]
        public DateTime CreateDate { get; set; }
        [Required]
        [Display(Name = "用戶組創建者")]
        public string CreateUser { get; set; }
        [Required]
        [Display(Name = "用戶組更新版本")]
        public int Updated { get; set; }
        [Display(Name = "用戶組更新者")]
        public string UpdateUser { get; set; }
        [Display(Name = "用戶組更新日期")]
        public DateTime? UpdateDate { get; set; }

    }
}