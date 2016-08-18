using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("funcmenu")]
    public class FuncMenu
    {
        [Key]
        [Display(Name = "權限ID")]
        public int ID { get; set; }
        [Required]
        [Display(Name = "權限名稱")]
        public string Name { get; set; }
        [Display(Name = "權限顯示")]
        public int ShowMenu { get; set; }
        [Display(Name = "權限URL")]
        public string URL { get; set; }
        [Required]
        [Display(Name = "權限狀態")]
        public int FuncmenuRight { get; set; }

        [Required]
        [Display(Name = "權限創造日期")]
        public DateTime CreateDate { get; set; }
        [Required]
        [Display(Name = "權限新增者")]
        public string CreateUser { get; set; }
        [Required]
        [Display(Name = "權限更新版本")]
        public int Updated { get; set; }
        [Display(Name = "權限更新者")]
        public string UpdateUser { get; set; }
        [Display(Name = "權限更新日期")]
        public DateTime? UpdateDate { get; set; }
    }
}