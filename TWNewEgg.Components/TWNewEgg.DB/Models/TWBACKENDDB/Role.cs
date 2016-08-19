using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("role")]
    public class Role
    {
        [Key]
        [Display(Name = "部門ID")]
        public int ID { get; set; }
        [Display(Name = "用戶組的部門ID(跨部門ID)")]
        public string GroupRolesID { get; set; }
        [Required]
        [Display(Name = "部門名稱")]
        public string Name { get; set; }
        [Display(Name = "部門描述")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "角色創造日期")]
        public DateTime CreateDate { get; set; }
        [Required]
        [Display(Name = "角色創建者")]
        public string CreateUser { get; set; }
        [Required]
        [Display(Name = "角色更新版本")]
        public int Updated { get; set; }
        [Display(Name = "角色更新者")]
        public string UpdateUser { get; set; }
        [Display(Name = "角色更新日期")]
        public DateTime? UpdateDate { get; set; }
    }
}