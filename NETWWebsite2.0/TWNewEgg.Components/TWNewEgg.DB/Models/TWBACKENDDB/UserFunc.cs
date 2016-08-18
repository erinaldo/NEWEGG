using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("userfunc")]
    public class UserFunc
    {
        [Key]
        [Display(Name = "個人權限ID")]
        public int ID { get; set; }
        [Required]
        [Display(Name = "個人權限用戶ID")]
        public int UserID { get; set; }
        [Required]
        [Display(Name = "個人權限功能ID")]
        public int FuncmenuID { get; set; }
        [Required]
        [Display(Name = "個人權限功能狀態")]
        public int UserFuncright { get; set; }

        [Required]
        [Display(Name = "個人權限創建日期")]
        public DateTime CreateDate { get; set; }
        [Required]
        [Display(Name = "個人權限創建者")]
        public string CreateUser { get; set; }
        [Required]
        [Display(Name = "個人權限版本")]
        public int Updated { get; set; }
        [Display(Name = "個人權限更新者")]
        public string UpdateUser { get; set; }
        [Display(Name = "個人權限更新日期")]
        public DateTime? UpdateDate { get; set; }
    }
}