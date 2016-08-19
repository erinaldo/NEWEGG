using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("SubCategory_NormalStore")]
    public class SubCategory_NormalStore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Title { get; set; }
        public int CategoryID { get; set; }
        public int StoreClass { get; set; }
        public int StyleClass { get; set; }
        public int ShowAll { get; set; }
        public int Showorder { get; set; }
        public string LogoImageURL { get; set; }
        public string StoreImageURL { get; set; }
        public string StoreImageLinkURL { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
