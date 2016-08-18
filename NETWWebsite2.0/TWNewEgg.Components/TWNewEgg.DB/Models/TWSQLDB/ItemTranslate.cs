using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("itemtranslate")]
    public class ItemTranslate
    {
        public ItemTranslate()
        {
            this.CreateDate = DateTime.UtcNow.AddHours(8);
            this.LastEditDate = new DateTime(1990, 1, 1);
        }

        public enum TranslateStatus
        {
            未翻譯 = 1,
            翻譯中 = 2,
            待審核 = 3,
            退回 = 4,
            完成 = 5,
            垃圾桶 = 900
        }

        public int ID { get; set; }
        public int ItemID { get; set; }
        public int ProductID { get; set; }
        public string DescriptionTW { get; set; }
        public string HasError { get; set; }
        public int Status { get; set; }
        public string ItemName { get; set; }
        public string CategoryL1 { get; set; }
        public int CategoryIDL1 { get; set; }
        public string CategoryL2 { get; set; }
        public int CategoryIDL2 { get; set; }
        public string CategoryL3 { get; set; }
        public int CategoryIDL3 { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string LastEditUser { get; set; }
        public DateTime LastEditDate { get; set; }
        public string ApproveUser { get; set; }
        [NotMapped]
        public bool IsError
        {
            get
            {
                if (this.HasError.ToLower() == "y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    this.HasError = "Y";
                }
                else
                {
                    this.HasError = "N";
                }
            }
        }

        /*public void SetError(bool isError)
        {
            if (isError)
            {
                this.HasError = "Y";
            }
            else
            {
                this.HasError = "N";
            }
        }*/
    }
}
