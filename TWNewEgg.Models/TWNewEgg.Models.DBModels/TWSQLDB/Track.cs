using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("track")]
    public class Track
    {
        public enum TrackStatus
        {
            購物車 = 0,
            追蹤清單 = 1,
            國內購物車加價商品 = 100,
            海外購物車加價商品 = 101,
            任選館購物車加價商品 = 102
        }
        public enum TrackCategoryType
        {
            一般類別 = 0,
            任選館 = 2
        }
        [Key]
        public int ID { get; set; }
        public int ACCID { get; set; }
        public int ItemID { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public int? CategoryID { get; set; }
        public int? CategoryType { get; set; }
        public int? Qty { get; set; }
    }
}
