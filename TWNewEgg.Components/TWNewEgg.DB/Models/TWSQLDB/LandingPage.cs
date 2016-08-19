using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("landingpage")]
    public class LandingPage
    {
        public LandingPage()
        {
            this.ShowType = 0;
            this.Updated = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DisplayName("頁面名稱")]
        public string Name { get; set; }
        public string HtmlContext { get; set; }
        [DisplayName("主商品ItemID清單")]
        public string MainItem { get; set; }
        [DisplayName("主標題清單")]
        public string Title { get; set; }
        [DisplayName("廣告文案")]
        public string ADCopy { get; set; }
        public string ItemTitle { get; set; }
        [DisplayName("商品副標清單")]
        public string Slogen { get; set; }
        [DisplayName("商品特點說明清單")]
        public string ItemDesc { get; set; }
        public int ShowType { get; set; }
        public Nullable<int> ActionType { get; set; }
        //public string ImgSource { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyWord { get; set; }
        public string MetaDescription { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}