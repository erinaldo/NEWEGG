using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Account
{
    public class MemberDM
    {
        public enum SetSex
        {
            Male = 1,
            Female = 0
        }

        public int AccID { get; set; }

        [DisplayName("稱謂")]
        public Nullable<int> Sex { get; set; }

        [DisplayName("姓氏")]
        public string Lastname { get; set; }

        [DisplayName("名字")]
        public string Firstname { get; set; }

        [DisplayName("暱稱")]
        public string Nickname { get; set; }

        [DisplayName("First Name")]
        public string Firstname_en { get; set; }

        [DisplayName("Last Name")]
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

    }
}
