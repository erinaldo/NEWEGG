using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemAndSubCategoryMapping_NormalStore")]
    public class ItemAndSubCategoryMapping_NormalStore
    {
        public enum Style
        {
            Item = 1,
            Image = 2,
            Title = 3,
            TextGroup = 4,
            Logo = 5
        };
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int SubCategoryID { get; set; }
        public int Zone { get; set; }
        public int StyleClass { get; set; }
        public int Showorder { get; set; }
        public int ItemID { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string LinkURL { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string Discription { get; set; }
    }
}
