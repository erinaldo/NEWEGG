using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("user")]
    public class User
    {
        [Key]
        [Display(Name = "用戶ID")]
        public int ID { get; set; }
        [Required]
        [Display(Name = "用戶短名")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "用戶密碼")]
        [StringLength(100, ErrorMessage = " {0} 至少要 {2} 字元長。", MinimumLength = 6)]
        public string Password { get; set; }
        [Display(Name="顯示用戶名")]
        public string ShowName { get; set; }
        [Display(Name = "用戶的信箱")]
        public string Email { get; set; }
        [Display(Name = "用戶的部門ID")]
        public int? RoleID { get; set; }
        [Required]
        [Display(Name = "用戶的跨部門ID")]
        public int GrouprolesID { get; set; }

        [Required]
        [Display(Name = "用戶的創建日期")]
        public DateTime CreateDate { get; set; }
        [Required]
        [Display(Name = "用戶的創建者")]
        public string CreateUser { get; set; }
        [Required]
        [Display(Name = "用戶的更新版本")]
        public int Updated { get; set; }
        [Display(Name = "用戶的更新者")]
        public string UpdateUser { get; set; }
        [Display(Name = "用戶的更新日期")]
        public DateTime? UpdateDate { get; set; }
    }
}