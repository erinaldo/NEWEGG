using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("rolefunc")]
    public class RoleFunc
    {
        [Key]
        [Display(Name = "部門權限ID")]
        public int ID { get; set; }
        [Required]
        [Display(Name = "部門權限部門ID")]
        public int RoleID { get; set; }
        [Required]
        [Display(Name = "部門權限功能ID")]
        public int FuncmenuID { get; set; }
        [Required]
        [Display(Name = "部門權限狀態")]
        public int RoleFuncRight { get; set; }

        [Required]
        [Display(Name = "部門權限創建日期")]
        public DateTime CreateDate { get; set; }
        [Required]
        [Display(Name = "部門權限創建者")]
        public string CreateUser { get; set; }
        [Required]
        [Display(Name = "部門權限更新版本")]
        public int Updated { get; set; }
        [Display(Name = "部門權限更新者")]
        public string UpdateUser { get; set; }
        [Display(Name = "部門權限更新日期")]
        public DateTime? UpdateDate { get; set; }
    }
}