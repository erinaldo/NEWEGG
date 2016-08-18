using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("member")]
    public class Member
    {
        public Member()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
            ModifyDate = defaultDate;

        }

        public enum SetSex
        {
            Male = 1,
            Female =0
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public int AccID { get; set; }     
        
        [DisplayName("稱謂")]
        public Nullable<int> Sex { get; set; }

        [DisplayName("姓氏")]
        [MaxLength(20, ErrorMessage = "姓氏請勿輸入超過10個字元")]
        public string Lastname { get; set; }

        [DisplayName("名字")]
        [MaxLength(20, ErrorMessage = "名字請勿輸入超過10個字元")]
        public string Firstname { get; set; }

        [DisplayName("暱稱")]
        [MaxLength(10, ErrorMessage = "網路暱稱請勿輸入超過10個字元")]
        public string Nickname { get; set; }

        [DisplayName("First Name")]
        [MaxLength(30, ErrorMessage = "First Name請勿輸入超過30個字元")]
        public string Firstname_en { get; set; }

        [DisplayName("Last Name")]
        [MaxLength(30, ErrorMessage = "Last Name請勿輸入超過30個字元")]
        public string Lastname_en { get; set; }

        [DisplayName("生日")]
        public string Birthday { get; set; }

        [DisplayName("行動電話")]
        public string Mobile { get; set; }

        [DisplayName("市話區碼")]
        public string TelZip { get; set; }

        [DisplayName("市話")]
        public string TelDay { get; set; }

        [DisplayName("分機")]
        public string TelExtension { get; set; }

        [DisplayName("縣市")]
        public string Loc { get; set; }

        [DisplayName("郵遞區號")]
        public string Zip { get; set; }

        [DisplayName("地區")]
        public string Zipname { get; set; }
        
        [DisplayName("地址")]
        public string Address { get; set; }

        [DisplayName("英文地址")]
        public string Address_en { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        
    }
}